using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class ApproveIssuanceHandler : IRequestHandler<ApproveIssuanceQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();

        public ApproveIssuanceHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApproveIssuanceQuery request, CancellationToken cancellationToken)
        {
            int check = 0;
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null, null,
             "IssuanceDetail," +
             "IndentRequest," +
             "IssuanceDetail.IndentRequestDetail," +
             "IssuanceDetail.IndentRequestDetail.Item," +
             "IssuanceDetail.IndentRequestDetail.Item.ItemType," +
             "IssuanceDetail.IndentRequestDetail.Item.ItemType.SubCategory," +
             "IssuanceDetail.IndentRequestDetail.Item.ItemType.SubCategory.Category," +
             "IssuanceDetail.IndentRequestDetail.Item.ItemType.SubCategory.Category.CategoryStores"
             );

            var activeDetails = Issuance.IssuanceDetail.Where(x => x.IsActive);
            var storeGroups = activeDetails.GroupBy(x => x.IndentRequestDetail.Item.ItemType.SubCategory.Category.CategoryStores.FirstOrDefault()?.StoreId)
                .Where(g => g.Key.HasValue);

            decimal totalPrice = 0;
            decimal totalQuantity = 0;

            SaveTransactionCommand tCommand = new()
            {
                Date = DateTime.Now,
                ReferenceNumber = Issuance.Code,
                Remarks = "Issuance voucher against Issuance #  " + Issuance.Code,
                VoucherTypeId = 1,
                StatusId = 3,
            };

            List<SaveTransactionDetailCommand> tDCommandList = new();

            foreach (var storeGroup in storeGroups)
            {
                decimal totalAmount = 0;
                decimal totalQty = 0;

                foreach (var gItem in storeGroup)
                {
                    decimal itemTotalPrice = gItem.Rate * gItem.Quantity;
                    totalAmount += itemTotalPrice;
                    totalQty += gItem.Quantity;
                }

                totalPrice += totalAmount;
                totalQuantity += totalQty;
                var storeId = storeGroup.Key.Value;

                var store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(x => x.Id == storeId);
                var account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == store.Name);
                if (account == null)
                {
                    return new Tuple<long, string>(503, $"Account not found for Store : {store.Name}!");
                }

                SaveTransactionDetailCommand tDCCommand = new()
                {
                    AccountId = account.Id,
                    DepartmentId = Issuance.IndentRequest.DepartmentId,
                    ProjectId = Issuance.IndentRequest.ProjectId,
                    IsGroup = false,
                    DebitAmount = 0,
                    CreditAmount = totalAmount,
                    Quantity = totalQty
                };

                tDCommandList.Add(tDCCommand);
            }

            SaveTransactionDetailCommand tDbCommandGlAccount = new()
            {
                AccountId = Issuance.AccountId,
                DepartmentId = Issuance.IndentRequest.DepartmentId,
                ProjectId = Issuance.IndentRequest.ProjectId,
                IsGroup = false,
                DebitAmount = totalPrice,
                CreditAmount = 0,
                Quantity = totalQuantity
            };
            tDCommandList.Add(tDbCommandGlAccount);

            tCommand.TransactionDetails = tDCommandList;
            await SaveTransaction(tCommand);

            var updateIssuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            updateIssuance.StatusId = 3;
            updateIssuance.ApprovedDate = DateTime.Now;
            updateIssuance.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Issuance>().Update(updateIssuance);
            check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Issuance Approved Successful!");
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
            var voucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
                .GetOneAsync(y => y.IsActive == true
                    && y.CompanyId == sessionProvider.Session.CompanyId
                    && y.Id == request.VoucherTypeId, null, null);

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

            string fullCode;

            // Ensure unique in this session (memory)
            do
            {
                fullCode = $"{prefix}{serial.ToString().PadLeft(4, '0')}";
                serial++;
            }
            while (codes.Contains(fullCode));

            codes.Add(fullCode);
            return fullCode;
        }
    }
}
