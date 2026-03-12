using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterial.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class ApproveSaleMaterialHandler : IRequestHandler<ApproveSaleMaterialQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;

        public ApproveSaleMaterialHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApproveSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetFirstAsNoTrackingWithIdentityResolutionAsync(y => y.Id == request.Id, null, null,
                "SaleMaterialDetail," +
                "SaleMaterialDetail.Item," +
                "SaleMaterialDetail.Item.ItemType," +
                "SaleMaterialDetail.Item.ItemType.SubCategory," +
                "SaleMaterialDetail.Item.ItemType.SubCategory.Category," +
                "SaleMaterialDetail.Item.ItemType.SubCategory.Category.CategoryStores"
                );

            SaleMaterial.StatusId = 3;
            SaleMaterial.ModifiedDate = DateTime.Now;
            SaleMaterial.ModifiedById = sessionProvider.Session.LoggedInUserId;
            SaleMaterial.ApprovedDate = DateTime.Now;
            SaleMaterial.ApprovedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Context().Entry(SaleMaterial).Property(x => x.StatusId).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterial).Property(x => x.ModifiedDate).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterial).Property(x => x.ModifiedById).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterial).Property(x => x.ApprovedDate).IsModified = true;
            unitOfWork.Context().Entry(SaleMaterial).Property(x => x.ApprovedById).IsModified = true;

            var accountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.DealershipId == SaleMaterial.DealershipId);
            if (accountGroup == null)
            {
                //return new Tuple<long, string>(501, "GL Account not found against " + item.Order.Dealership.Name + "!"); // Tracking Issue
                var dealer = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == SaleMaterial.DealershipId);
                return new Tuple<long, string>(501, "GL Account not found against " + dealer.Name + "!");
            }

            //decimal totalPrice = 0;
            //foreach (var detail in SaleMaterial.SaleMaterialDetail.Where(x => x.IsActive))
            //{
            //    decimal itemTotalPrice = (decimal)(detail.Rate * detail.Quantity);
            //    totalPrice += itemTotalPrice;
            //}

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
                ReferenceNumber = SaleMaterial.Code,
                Remarks = "Amount deducted from customer against sale material no " + SaleMaterial.Code,
                VoucherTypeId = 6,
                StatusId = 3,
            };

            List<SaveTransactionDetailCommand> tDCommandList = new();

            var activeDetails = SaleMaterial.SaleMaterialDetail.Where(x => x.IsActive);

            var storeGroups = activeDetails
                .GroupBy(x => x.Item.ItemType.SubCategory.Category.CategoryStores.FirstOrDefault()?.StoreId)
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

                decimal storeTotalPrice = storeGroup.Sum(x => (decimal)(x.Quantity * x.Rate));
                decimal storeTotalQty = storeGroup.Sum(x => x.Quantity);

                SaveTransactionDetailCommand tDCCommand = new()
                {
                    AccountId = account.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = false,
                    DebitAmount = 0,
                    CreditAmount = storeTotalPrice,
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
                DebitAmount = totalPrice,
                CreditAmount = 0,
                Quantity = SaleMaterial.SaleMaterialDetail.Where(x => x.IsActive).Sum(y => y.Quantity)
            };
            tDCommandList.Add(tDDCommand);


            tCommand.TransactionDetails = tDCommandList;
            await SaveTransaction(tCommand);

            //unitOfWork.Repository<Entities.Models.SaleMaterial>().Update(SaleMaterial);

            await unitOfWork.SaveChangesAsync();
            return new Tuple<long, string>(200, "Approved!");
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

            _Transaction.TransactionDocuments.ForEach(y =>
            {
                y.CreatedDate = DateTime.Now;
                y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
            });

            unitOfWork.Repository<Entities.Models.Transaction>().Add(_Transaction);
            return 200;
        }

        public async Task<string> GetCode(GetTransactionCodeQuery request)
        {
            var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetOneAsync(y => y.IsActive == true
                    && y.CompanyId == sessionProvider.Session.CompanyId
                    && y.Id == request.VoucherTypeId, null, null);

            string _TransactionCode = "";
            string yearMonth = request.VoucherDate.Year.ToString("") + request.VoucherDate.Month.ToString("D2"); // YYYYMM Format

            // Fetch last transaction for the same Voucher Type & Year-Month
            Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> OrderByDesc =
                query => query.OrderByDescending(x => x.Code);

            var lastTransaction = await unitOfWork.Repository<Entities.Models.Transaction>()
                .GetFirstAsNoTrackingAsync(y => y.IsActive == true
                    && y.CompanyId == sessionProvider.Session.CompanyId
                    && y.VoucherTypeId == request.VoucherTypeId
                    && y.Code.StartsWith(VoucherType.Code + yearMonth),
                    OrderByDesc, null);

            // Extract last 4-digit serial and increment
            int newSerial = 1;
            if (lastTransaction != null)
            {
                string lastSerialStr = lastTransaction.Code.Substring((VoucherType.Code + yearMonth).Length);
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    newSerial = lastSerial + 1;
                }
            }

            // Format the new transaction code
            _TransactionCode = newSerial.ToString().PadLeft(4, '0'); // 4-digit serial

            return $"{VoucherType.Code}{yearMonth}{_TransactionCode}";
        }


    }
}
