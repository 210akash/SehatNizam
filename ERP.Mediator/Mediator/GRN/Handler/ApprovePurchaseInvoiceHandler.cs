using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class ApprovePurchaseInvoiceHandler : IRequestHandler<ApprovePurchaseInvoiceQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();

        public ApprovePurchaseInvoiceHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApprovePurchaseInvoiceQuery request, CancellationToken cancellationToken)
        {
            int check = 0;
            var gRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null, null,
              "GRNDetail," +
              "GRNDetail.CostSheet," +
              "GRNDetail.CostSheet.CostSheetDetail," +
              "Inspection," +
              "Inspection.IGP," +
              "Inspection.IGP.PurchaseOrder," +
              "GRNDetail.InspectionDetail," +
              "GRNDetail.InspectionDetail.IGPDetail," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores"
              );

            var GRNUpdate = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var vendor = gRN.GRNDetail.FirstOrDefault().InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor;

            var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
            if (Accountsdepartment == null)
            {
                return new Tuple<long, string>(502, "Accounts Department not found!");
            }

            SaveTransactionCommand tCommand = new()
            {
                Date = DateTime.Now,
                ReferenceNumber = gRN.InvoiceNo,
                Remarks = "Invoice voucher against Invoice # " + gRN.InvoiceNo + " a/c " + vendor.Name,
                VoucherTypeId = 8,
                StatusId = 3,
            };

            List<SaveTransactionDetailCommand> tDCommandList = new();

            var activeDetails = gRN.GRNDetail.Where(x => x.IsActive);

            var storeGroups = activeDetails
                .GroupBy(x => x.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores.FirstOrDefault()?.StoreId)
                .Where(g => g.Key.HasValue);

            decimal totalPrice = 0;
            decimal totalSaleTax = 0;
            decimal totalFed = 0;
            decimal totalQuantity = 0;

            foreach (var storeGroup in storeGroups)
            {
                decimal totalAmount = 0;

                foreach (var gItem in storeGroup)
                {
                    decimal itemTotalPrice = gItem.Received * gItem.InspectionDetail.IGPDetail.PurchaseOrderDetail.UnitRate;
                    decimal itemFed = (gItem.InspectionDetail.IGPDetail.PurchaseOrderDetail.FED / gItem.InspectionDetail.IGPDetail.PurchaseOrderDetail.Quantity) * gItem.Received;
                    decimal itemSaleTax = (gItem.InspectionDetail.IGPDetail.PurchaseOrderDetail.GST / gItem.InspectionDetail.IGPDetail.PurchaseOrderDetail.Quantity) * gItem.Received;

                    totalAmount += itemTotalPrice;
                    totalFed += itemFed;
                    totalSaleTax += itemSaleTax;
                    totalQuantity += gItem.Received;
                }

                totalPrice += totalAmount;
            }

            var stockAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Stock Received But Not Billed");
            if (stockAccount == null)
            {
                return new Tuple<long, string>(502, "Account Goods Received But Not Billed not found!");
            }

            var vendorAccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == vendor.Id);
            if (vendorAccount == null)
            {
                return new Tuple<long, string>(502, "Vendor Account not found!");
            }

            var wHTAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Withholding Tax Payable");
            if (wHTAccount == null)
            {
                return new Tuple<long, string>(502, "WHT Account not found!");
            }

            SaveTransactionDetailCommand tDsCommand = new()
            {
                AccountId = stockAccount.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = false,
                DebitAmount = totalPrice + totalFed + totalSaleTax,
                CreditAmount = 0,
                Quantity = totalQuantity
            };
            tDCommandList.Add(tDsCommand);

            SaveTransactionDetailCommand tDvCommand = new()
            {
                AccountGroupId = vendorAccount.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = true,
                DebitAmount = 0,
                CreditAmount = totalPrice + totalFed + totalSaleTax,
                Quantity = totalQuantity
            };
            tDCommandList.Add(tDvCommand);

            tCommand.TransactionDetails = tDCommandList;
            await SaveTransaction(tCommand);

            #region WHT Journal Voucher 

            if (gRN.WHTPercentage != null)
            {
                SaveTransactionCommand tCommand2 = new()
                {
                    Date = DateTime.Now,
                    ReferenceNumber = gRN.InvoiceNo,

                    Remarks = "Tax deducted against Inv# " + gRN.InvoiceNo + "a/c " + vendor.Name,

                    VoucherTypeId = 1,
                    StatusId = 3,
                };
                List<SaveTransactionDetailCommand> tDCommandList2 = new();

                decimal? wHtAmount = (totalPrice * gRN.WHTPercentage) / 100;
                SaveTransactionDetailCommand tDsCommand2 = new()
                {
                    AccountGroupId = vendorAccount.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = true,
                    DebitAmount = (decimal)wHtAmount,
                    CreditAmount = 0,
                    Quantity = totalQuantity
                };
                tDCommandList2.Add(tDsCommand2);

                SaveTransactionDetailCommand tDvCommand2 = new()
                {
                    AccountId = wHTAccount.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = false,
                    DebitAmount = 0,
                    CreditAmount = (decimal)wHtAmount,
                    Quantity = totalQuantity
                };
                tDCommandList2.Add(tDvCommand2);

                tCommand2.TransactionDetails = tDCommandList2;
                await SaveTransaction(tCommand2);
            }

            #endregion

            GRNUpdate.InvoiceStatusId = 3;
            GRNUpdate.InvoiceApprovedDate = DateTime.Now;
            GRNUpdate.InvoiceApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.GRN>().Update(GRNUpdate);
            check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Purchase Invoice Approved Successful!");
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
