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
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class ApprovePurchaseReturnHandler : IRequestHandler<ApprovePurchaseReturnQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();

        public ApprovePurchaseReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApprovePurchaseReturnQuery request, CancellationToken cancellationToken)
        {
            var PurchaseReturn = await unitOfWork
             .Repository<Entities.Models.PurchaseReturn>()
             .GetFirstAsync(
               y => y.Id == request.Id,
                 null, null,
                 "GRN," +
                 "GRN.Inspection," +
                 "GRN.Inspection.IGP," +
                 "GRN.Inspection.IGP.PurchaseOrder," +
                 "PurchaseReturnDetail.GRNDetail," +
                 "PurchaseReturnDetail.GRNDetail.CostSheet," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category," +
                 "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores");

            var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
            if (Accountsdepartment == null)
            {
                return new Tuple<long, string>(502, "Accounts Department not found!");
            }

            if (PurchaseReturn != null && PurchaseReturn.PurchaseReturnDetail.Any(y => y.GRNDetail.CostSheetId.HasValue && y.GRNDetail.CostSheetId > 0))
            {
                var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetFirstAsNoTrackingAsync(x => x.Id == PurchaseReturn.GRN.Inspection.IGP.PurchaseOrder.VendorId);
                if (vendor == null)
                {
                    //return new Tuple<long, string>(501, "GL Account not found against " + item.Order.Dealership.Name + "!"); // Tracking Issue
                    return new Tuple<long, string>(501, "Vendor not found !");
                }

                var vendormaterialaccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == vendor.Id && x.Name.Contains("Material"));
                if (vendormaterialaccount == null)
                {
                    return new Tuple<long, string>(501, "GL Account (Material) not found against " + vendor.Name + "!");
                }

                var vendorTollFillingaccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == vendor.Id && x.Name.Contains("Toll Filling"));
                if (vendorTollFillingaccount == null)
                {
                    return new Tuple<long, string>(501, "GL Account (Toll Filling) not found against " + vendor.Name + "!");
                }

                var astaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0102090002");
                if (astaccount == null)
                {
                    return new Tuple<long, string>(503, "Advance Sales Tax account not found!");
                }

                var afedaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0102090003");
                if (afedaccount == null)
                {
                    return new Tuple<long, string>(503, "Advance FED account not found!");
                }

                var fgsaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0102120003");
                if (fgsaccount == null)
                {
                    return new Tuple<long, string>(503, "Finished Goods Store account not found!");
                }

                var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Purchase");
                if (department == null)
                {
                    return new Tuple<long, string>(502, "Purchase Department not found!");
                }

                var Storedepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Store");
                if (Storedepartment == null)
                {
                    return new Tuple<long, string>(502, "Store Department not found!");
                }

                var purchaseReturnItems = PurchaseReturn.PurchaseReturnDetail.Where(x => x.IsActive && x.GRNDetail?.CostSheetId > 0);

                foreach (var item in purchaseReturnItems)
                {
                    decimal prQuantity = item.Quantity;

                    // --- Per-unit values ---
                    decimal materialCostPerUnit = item.GRNDetail.CostSheet.TMaterialCost / item.GRNDetail.CostSheet.Quantity;
                    decimal tollFillingPerUnit = item.GRNDetail.CostSheet.TollFillRate; // or TFillingPerPet
                    decimal advSaleTaxPerUnit = (tollFillingPerUnit * item.GRNDetail.CostSheet.AdvSaleTaxPer) / 100;
                    decimal advFEDPerUnit = (tollFillingPerUnit * item.GRNDetail.CostSheet.AdvFEDPer) / 100;

                    // --- Totals for GRN quantity ---
                    decimal TotalMaterialCost = materialCostPerUnit * prQuantity;
                    decimal TotalTollFilling = tollFillingPerUnit * prQuantity;
                    decimal TotalAdvSaleTax = advSaleTaxPerUnit * prQuantity;
                    decimal TotalAdvFED = advFEDPerUnit * prQuantity;

                    // --- Cost of Production ---
                    decimal CostOfProduction = TotalMaterialCost + TotalTollFilling;

                    SaveTransactionCommand tCommand = new()
                    {
                        Date = DateTime.Now,
                        ReferenceNumber = "PR-" + PurchaseReturn.Code,
                        Remarks = " Purchase return voucher against Purchase Return #  " + PurchaseReturn.Code + " , Item " + item.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.Name,
                        VoucherTypeId = 1,
                        StatusId = 3,
                    };
                    List<SaveTransactionDetailCommand> tDCommandList = new();

                    // 1- Vendor Account Material Debit
                    SaveTransactionDetailCommand tDDCommand = new()
                    {
                        AccountGroupId = vendormaterialaccount.Id,
                        DepartmentId = department.Id,
                        ProjectId = 3,
                        IsGroup = true,
                        DebitAmount = TotalMaterialCost,
                        CreditAmount = 0,
                        Quantity = prQuantity
                    };
                    tDCommandList.Add(tDDCommand);

                    // 2- Vendor Account Toll Filling Debit
                    SaveTransactionDetailCommand tDTFCommand = new()
                    {
                        AccountGroupId = vendorTollFillingaccount.Id,
                        DepartmentId = department.Id,
                        ProjectId = 3,
                        IsGroup = true,
                        DebitAmount = TotalTollFilling + TotalAdvSaleTax + TotalAdvFED,
                        CreditAmount = 0,
                        Quantity = prQuantity
                    };
                    tDCommandList.Add(tDTFCommand);

                    // 3- Finished Goods Store Credit
                    SaveTransactionDetailCommand tTMcommand = new()
                    {
                        AccountId = fgsaccount.Id,
                        DepartmentId = Storedepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        CreditAmount = CostOfProduction,
                        DebitAmount = 0,
                        Quantity = prQuantity
                    };
                    tDCommandList.Add(tTMcommand);

                    // 4- Advance Sales Tax Credit
                    SaveTransactionDetailCommand tTPcommand = new()
                    {
                        AccountId = astaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = 0,
                        CreditAmount = TotalAdvSaleTax,
                        Quantity = prQuantity
                    };
                    tDCommandList.Add(tTPcommand);

                    // 5- Advance FED  Credit
                    SaveTransactionDetailCommand tDMcommand = new()
                    {
                        AccountId = afedaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = 0,
                        CreditAmount = TotalAdvFED,
                        Quantity = prQuantity
                    };
                    tDCommandList.Add(tDMcommand);

                    tCommand.TransactionDetails = tDCommandList;
                    await SaveTransaction(tCommand);
                }
            }
            else
            {
                SaveTransactionCommand tCommand = new()
                {
                    Date = DateTime.Now,
                    ReferenceNumber = "PR-" + PurchaseReturn.Code,
                    Remarks = " Purchase return voucher against Purchase Return Invoice #  " + PurchaseReturn.GRN.InvoiceNo,
                    VoucherTypeId = 1,
                    StatusId = 3,
                };

                List<SaveTransactionDetailCommand> tDCommandList = new();
                var activeDetails = PurchaseReturn.PurchaseReturnDetail.Where(x => x.IsActive);
                var storeGroups = activeDetails
                    .GroupBy(x => x.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores.FirstOrDefault()?.StoreId)
                    .Where(g => g.Key.HasValue);

                decimal totalPrice = 0;
                decimal totalQuantity = 0;
                decimal totalSaleTax = 0;

                foreach (var storeGroup in storeGroups)
                {
                    decimal totalAmount = 0;
                    foreach (var gItem in storeGroup)
                    {
                        decimal itemTotalPrice = gItem.Quantity * gItem.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.UnitRate;
                        decimal itemSaleTax = (gItem.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.GST / gItem.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.Quantity) * gItem.Quantity;
                        totalAmount += itemTotalPrice;
                        totalQuantity += gItem.Quantity;
                        totalSaleTax += itemSaleTax;
                    }

                    totalPrice += totalAmount;
                    var storeId = storeGroup.Key.Value;

                    var store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(x => x.Id == storeId);
                    var storeAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == store.Name);
                    if (storeAccount == null)
                    {
                        return new Tuple<long, string>(503, $"Account not found for Store : {store.Name}!");
                    }

                    SaveTransactionDetailCommand tDCCommand = new()
                    {
                        AccountId = storeAccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = 0,
                        CreditAmount = totalAmount,
                        Quantity = totalQuantity
                    };

                    tDCommandList.Add(tDCCommand);
                }

                var saleTaxAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Advance Sales Tax");
                if (saleTaxAccount == null)
                {
                    return new Tuple<long, string>(502, "Account Advance Sales Tax not found!");
                }

                var vendorAccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == PurchaseReturn.GRN.Inspection.IGP.PurchaseOrder.VendorId);
                if (vendorAccount == null)
                {
                    return new Tuple<long, string>(502, "Vendor Account not found!");
                }

                SaveTransactionDetailCommand tDvCommand2 = new()
                {
                    AccountId = saleTaxAccount.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = false,
                    DebitAmount = 0,
                    CreditAmount = totalSaleTax,
                    Quantity = totalQuantity
                };
                tDCommandList.Add(tDvCommand2);
                totalPrice += totalSaleTax;

                SaveTransactionDetailCommand tDbCommand = new()
                {
                    AccountGroupId = vendorAccount.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = true,
                    DebitAmount = totalPrice,
                    CreditAmount = 0,
                    Quantity = totalQuantity
                };
                tDCommandList.Add(tDbCommand);

                tCommand.TransactionDetails = tDCommandList;
                await SaveTransaction(tCommand);

            }

            var PurchaseReturnUpdate = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetFirstAsync(y => y.Id == request.Id);
            PurchaseReturnUpdate.StatusId = 3;
            PurchaseReturnUpdate.ApprovedDate = DateTime.Now;
            PurchaseReturnUpdate.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseReturn>().Update(PurchaseReturnUpdate);
            int check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Purchase Return Approved Successful!");
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
