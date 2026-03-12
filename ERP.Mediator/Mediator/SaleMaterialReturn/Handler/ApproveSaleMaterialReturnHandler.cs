using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Stripe.Terminal;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class ApproveSaleMaterialReturnHandler : IRequestHandler<ApproveSaleMaterialReturnQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;

        public ApproveSaleMaterialReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApproveSaleMaterialReturnQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterialReturn = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetFirstAsNoTrackingWithIdentityResolutionAsync(y => y.Id == request.Id, null, null,
                "SaleMaterial," +
                "SaleMaterialReturnDetail," +
                "SaleMaterialReturnDetail.SaleMaterialDetail," +
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item," +
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item.ItemType," +
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item.ItemType.SubCategory," +
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item.ItemType.SubCategory.Category," +
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item.ItemType.SubCategory.Category.CategoryStores"
                );

            SaleMaterialReturn.StatusId = 3;
            SaleMaterialReturn.ModifiedDate = DateTime.Now;
            SaleMaterialReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
            SaleMaterialReturn.ApprovedDate = DateTime.Now;
            SaleMaterialReturn.ApprovedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Context().Entry(SaleMaterialReturn).Property(x => x.StatusId).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterialReturn).Property(x => x.ModifiedDate).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterialReturn).Property(x => x.ModifiedById).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterialReturn).Property(x => x.ApprovedDate).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterialReturn).Property(x => x.ApprovedById).IsModified = true;

            var accountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.DealershipId == SaleMaterialReturn.SaleMaterial.DealershipId);
            if (accountGroup == null)
            {
                //return new Tuple<long, string>(501, "GL Account not found against " + item.Order.Dealership.Name + "!"); // Tracking Issue
                var dealer = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == SaleMaterialReturn.SaleMaterial.DealershipId);
                return new Tuple<long, string>(501, "GL Account not found against " + dealer.Name + "!");
            }

            var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Sale");
            if (department == null)
            {
                return new Tuple<long, string>(502, "Department not found!");
            }

            var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
            if (Accountsdepartment == null)
            {
                return new Tuple<long, string>(502, "Accounts Department not found!");
            }

            SaveTransactionCommand tCommand = new()
            {
                Date = DateTime.Now,
                ReferenceNumber = SaleMaterialReturn.Code,
                Remarks = "Credit given to customer" + accountGroup.Name + " against sale material return no " + SaleMaterialReturn.Code,
                VoucherTypeId = 9,
                StatusId = 3,
            };

            List<SaveTransactionDetailCommand> tDCommandList = new();

            var activeDetails = SaleMaterialReturn.SaleMaterialReturnDetail.Where(x => x.IsActive);

            var storeGroups = activeDetails
                .GroupBy(x => x.SaleMaterialDetail.Item.ItemType.SubCategory.Category.CategoryStores.FirstOrDefault()?.StoreId)
                .Where(g => g.Key.HasValue);

            decimal totalPrice = 0;
            foreach (var storeGroup in storeGroups)
            {
                var storeId = storeGroup.Key.Value;

                var store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(x => x.Id == storeId);
                var account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == store.Name);
                if (account == null)
                {
                    return new Tuple<long, string>(503, $"Account not found for Store : {store.Name}!");
                }

                decimal storeTotalPrice = storeGroup.Sum(x => (decimal)(x.Quantity * x.SaleMaterialDetail.Rate));
                decimal storeTotalQty = storeGroup.Sum(x => x.Quantity);

                SaveTransactionDetailCommand tDCCommand = new()
                {
                    AccountId = account.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = false,
                    DebitAmount = storeTotalPrice,
                    CreditAmount = 0,
                    Quantity = storeTotalQty
                };

                tDCommandList.Add(tDCCommand);
                totalPrice += storeTotalPrice;
            }

            SaveTransactionDetailCommand tDDCommand = new()
            {
                AccountGroupId = accountGroup.Id,
                DepartmentId = department.Id,
                ProjectId = 3,
                IsGroup = true,
                DebitAmount = 0,
                CreditAmount = totalPrice,
                Quantity = SaleMaterialReturn.SaleMaterialReturnDetail.Where(x => x.IsActive).Sum(y => y.Quantity)
            };
            tDCommandList.Add(tDDCommand);

            tCommand.TransactionDetails = tDCommandList;
            await SaveTransaction(tCommand);
            int check = await unitOfWork.SaveChangesAsync();
            if (check > 0)
            {
                return new Tuple<long, string>(200, "Sale Material Return Approved Successful!");
            }
            else
            {
                return new Tuple<long, string>(500, "Error Approving, Please contact system admin!");
            }
        }

        async Task<long> SaveTransaction(SaveTransactionCommand request)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            GetTransactionCodeQuery getTransactionCodeQuery = new GetTransactionCodeQuery(request.VoucherTypeId, request.Date);
            var _newCode = await GetCode(getTransactionCodeQuery);
            request.Code = _newCode;

            var _Transaction = mapper.Map<Entities.Models.Transaction>(request);
            _Transaction.CompanyId = sessionProvider.Session.CompanyId;
            _Transaction.CreatedById = sessionProvider.Session.LoggedInUserId;
            _Transaction.ProcessedById = sessionProvider.Session.LoggedInUserId;
            _Transaction.ApprovedById = sessionProvider.Session.LoggedInUserId;
            _Transaction.CreatedDate = DateTime.Now;
            _Transaction.ProcessedDate = DateTime.Now;
            _Transaction.ApprovedDate = DateTime.Now;

            _Transaction.TransactionDetails.ForEach(y =>
            {
                y.CreatedDate = DateTime.Now;
                y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
            });

            unitOfWork.Repository<Entities.Models.Transaction>().Add(_Transaction);
            return 200;
        }

        public async Task<string> GetCode(GetTransactionCodeQuery request)
        {
            var voucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetOneAsync(y => y.IsActive == true
                    && y.CompanyId == sessionProvider.Session.CompanyId
                    && y.Id == request.VoucherTypeId, null, null);

            string _TransactionCode = "";
            string yearMonth = request.VoucherDate.Year.ToString() + request.VoucherDate.Month.ToString("D2");
            string prefix = voucherType.Code + yearMonth;

            Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> orderByDesc =
                query => query.OrderByDescending(x => x.Code);

            var lastTransaction = await unitOfWork.Repository<Entities.Models.Transaction>()
                .GetFirstAsNoTrackingAsync(
                    y => y.IsActive == true
                        && y.CompanyId == sessionProvider.Session.CompanyId
                        && y.VoucherTypeId == request.VoucherTypeId
                        && y.Code.StartsWith(prefix),
                    orderByDesc, null);

            int serial = 1;
            if (lastTransaction != null)
            {
                string lastSerialStr = lastTransaction.Code.Substring(prefix.Length);
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    serial = lastSerial + 1;
                }
            }

            _TransactionCode = serial.ToString().PadLeft(4, '0'); // 4-digit serial
            return $"{voucherType.Code}{yearMonth}{_TransactionCode}";
        }
    }
}
