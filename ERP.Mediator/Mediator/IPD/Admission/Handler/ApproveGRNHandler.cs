using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Extensions;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class ApproveGRNHandler : IRequestHandler<ApproveGRNQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();

        public ApproveGRNHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApproveGRNQuery request, CancellationToken cancellationToken)
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
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category," +
              "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.ItemType.SubCategory.Category.CategoryStores"
              );

            var GRNUpdate = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            #region Voucher

            if (gRN != null && gRN.GRNDetail.FirstOrDefault().CostSheetId != null && gRN.GRNDetail.FirstOrDefault().CostSheetId != 0)
            {
                foreach (var item in gRN.GRNDetail.Where(y=>y.IsActive))
                {
                    if (item.CostSheetId != null && item.CostSheetId != 0)
                    {
                        #region Finnish Goods Store

                        var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetFirstAsNoTrackingAsync(x => x.Id == gRN.Inspection.IGP.PurchaseOrder.VendorId);

                        var vendormaterialaccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == gRN.Inspection.IGP.PurchaseOrder.VendorId && x.Name.Contains("Material"));
                        if (vendormaterialaccount == null)
                        {
                            //return new Tuple<long, string>(501, "GL Account not found against " + item.Order.Dealership.Name + "!"); // Tracking Issue
                            return new Tuple<long, string>(501, "GL Account (Material) not found against " + vendor.Name + "!");
                        }

                        var vendorTollFillingaccount = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.VendorId == gRN.Inspection.IGP.PurchaseOrder.VendorId && x.Name.Contains("Toll Filling"));
                        if (vendorTollFillingaccount == null)
                        {
                            //return new Tuple<long, string>(501, "GL Account not found against " + item.Order.Dealership.Name + "!"); // Tracking Issue
                            return new Tuple<long, string>(501, "GL Account not found against " + vendor.Name + "!");
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

                        var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
                        if (Accountsdepartment == null)
                        {
                            return new Tuple<long, string>(502, "Accounts Department not found!");
                        }

                        var fgsaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0102120003");
                        if (fgsaccount == null)
                        {
                            return new Tuple<long, string>(503, "Finished Goods Store account not found!");
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

                        decimal grnQuantity = item.Received;
                        // --- Per-unit values ---
                        decimal materialCostPerUnit = item.CostSheet.TMaterialCost / item.CostSheet.Quantity;
                        decimal tollFillingPerUnit = item.CostSheet.TollFillRate; // or TFillingPerPet
                        decimal advSaleTaxPerUnit = (tollFillingPerUnit * item.CostSheet.AdvSaleTaxPer) / 100;
                        decimal advFEDPerUnit = (tollFillingPerUnit * item.CostSheet.AdvFEDPer) / 100;

                        // --- Totals for GRN quantity ---
                        decimal TotalMaterialCost = materialCostPerUnit * grnQuantity;
                        decimal TotalTollFilling = tollFillingPerUnit * grnQuantity;
                        decimal TotalAdvSaleTax = advSaleTaxPerUnit * grnQuantity;
                        decimal TotalAdvFED = advFEDPerUnit * grnQuantity;

                        // --- Cost of Production ---
                        decimal CostOfProduction = TotalMaterialCost + TotalTollFilling;

                        SaveTransactionCommand tCommand = new()
                        {
                            Date = DateTime.Now,
                            ReferenceNumber = gRN.Code,
                            Remarks = "Purchase voucher against GRN # " + gRN.Code + " , Item " + item.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.Name,
                            VoucherTypeId = 7,
                            StatusId = 3,
                            GRNDetailId = item.Id
                        };

                        List<SaveTransactionDetailCommand> tDCommandList = new();
                        // 1- Vendor Account Material Credit
                        SaveTransactionDetailCommand tDDCommand = new()
                        {
                            AccountGroupId = vendormaterialaccount.Id,
                            DepartmentId = department.Id,
                            ProjectId = 3,
                            IsGroup = true,
                            DebitAmount = 0,
                            CreditAmount = TotalMaterialCost,
                            Quantity = grnQuantity
                        };
                        tDCommandList.Add(tDDCommand);

                        // 2- Vendor Account Toll Filling Credit
                        SaveTransactionDetailCommand tDTFCommand = new()
                        {
                            AccountGroupId = vendorTollFillingaccount.Id,
                            DepartmentId = department.Id,
                            ProjectId = 3,
                            IsGroup = true,
                            DebitAmount = 0,
                            CreditAmount = TotalTollFilling + TotalAdvSaleTax + TotalAdvFED,
                            Quantity = grnQuantity
                        };
                        tDCommandList.Add(tDTFCommand);

                        // 3- Finished Goods Store   Debit
                        SaveTransactionDetailCommand tTMcommand = new()
                        {
                            AccountId = fgsaccount.Id,
                            DepartmentId = Storedepartment.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            DebitAmount = CostOfProduction,
                            CreditAmount = 0,
                            Quantity = grnQuantity
                        };
                        tDCommandList.Add(tTMcommand);

                        // 4- Advance Sales Tax
                        SaveTransactionDetailCommand tTPcommand = new()
                        {
                            AccountId = astaccount.Id,
                            DepartmentId = Accountsdepartment.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            DebitAmount = TotalAdvSaleTax,
                            CreditAmount = 0,
                            Quantity = grnQuantity
                        };
                        tDCommandList.Add(tTPcommand);

                        // 5- Advance FED  Debit
                        SaveTransactionDetailCommand tDMcommand = new()
                        {
                            AccountId = afedaccount.Id,
                            DepartmentId = Accountsdepartment.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            DebitAmount = TotalAdvFED,
                            CreditAmount = 0,
                            Quantity = grnQuantity
                        };
                        tDCommandList.Add(tDMcommand);

                        tCommand.TransactionDetails = tDCommandList;
                       await SaveTransaction(tCommand);

                        #endregion
                    }
                }
            }
            else
            {
                #region Other Stores

                var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
                if (Accountsdepartment == null)
                {
                    return new Tuple<long, string>(502, "Accounts Department not found!");
                }

                SaveTransactionCommand tCommand = new()
                {
                    Date = DateTime.Now,
                    ReferenceNumber = gRN.Code,
                    Remarks = "Purchase voucher against GRN #  " + gRN.Code,
                    VoucherTypeId = 7,
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
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = totalAmount,
                        CreditAmount = 0,
                        Quantity = totalQuantity
                    };

                    tDCommandList.Add(tDCCommand);
                }

                var saleTaxAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Advance Sales Tax");
                if (saleTaxAccount == null)
                {
                    return new Tuple<long, string>(502, "Account Advance Sales Tax not found!");
                }

                var fedAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Advance FED");
                if (fedAccount == null)
                {
                    return new Tuple<long, string>(502, "Account Advance FED not found!");
                }

                var stockAccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Name == "Stock Received But Not Billed");
                if (stockAccount == null)
                {
                    return new Tuple<long, string>(502, "Account Goods Received But Not Billed not found!");
                }

                if(totalSaleTax > 0)
                {
                    SaveTransactionDetailCommand tDsCommand = new()
                    {
                        AccountId = saleTaxAccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = totalSaleTax,
                        CreditAmount = 0,
                        Quantity = totalQuantity
                    };
                    tDCommandList.Add(tDsCommand);
                }

                if(totalFed > 0)
                {
                    SaveTransactionDetailCommand tDfCommand = new()
                    {
                        AccountId = fedAccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = totalFed,
                        CreditAmount = 0,
                        Quantity = totalQuantity
                    };
                    tDCommandList.Add(tDfCommand);
                }

                SaveTransactionDetailCommand tDbCommand = new()
                {
                    AccountId = stockAccount.Id,
                    DepartmentId = Accountsdepartment.Id,
                    ProjectId = 3,
                    IsGroup = false,
                    DebitAmount = 0,
                    CreditAmount = totalPrice + totalFed + totalSaleTax,
                    Quantity = totalQuantity
                };
                tDCommandList.Add(tDbCommand);

                tCommand.TransactionDetails = tDCommandList;
                await SaveTransaction(tCommand);

                string _InvoiceNo = "";
                if (await unitOfWork.Repository<Entities.Models.GRN>().GetExistsAsync(o => o.IsActive == true && o.StatusId == 3))
                {
                    Func<IQueryable<Entities.Models.GRN>, IOrderedQueryable<Entities.Models.GRN>> OrderByDesc = query => query.OrderByDescending(x => x.InvoiceNo);
                    var GRNCode = await unitOfWork.Repository<Entities.Models.GRN>().GetOneAsync(y => y.IsActive == true && y.StatusId == 3, OrderByDesc, null);
                    int No = Convert.ToInt32(GRNCode.InvoiceNo) + 1;
                    _InvoiceNo = No.ToString().PadLeft(7, '0');
                }
                else
                    _InvoiceNo = "0000001";

                GRNUpdate.InvoiceNo = _InvoiceNo;
                #endregion
            }

            #endregion

            GRNUpdate.StatusId = 3;
            GRNUpdate.ApprovedDate = DateTime.Now;
            GRNUpdate.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.GRN>().Update(GRNUpdate);
            check = await unitOfWork.SaveChangesAsync();

            if (check > 0)
            {
                return new Tuple<long, string>(200, "GRN Approved Successful!");
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

        //public async Task<string> GetCode(GetTransactionCodeQuery request)
        //{
        //    var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>()
        //        .GetOneAsync(y => y.IsActive == true
        //            && y.CompanyId == sessionProvider.Session.CompanyId
        //            && y.Id == request.VoucherTypeId, null, null);

        //    string _TransactionCode = "";
        //    string yearMonth = request.VoucherDate.Year.ToString("") + request.VoucherDate.Month.ToString("D2"); // YYYYMM Format

        //    // Fetch last transaction for the same Voucher Type & Year-Month
        //    Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> OrderByDesc =
        //        query => query.OrderByDescending(x => x.Code);

        //    var lastTransaction = await unitOfWork.Repository<Entities.Models.Transaction>()
        //        .GetFirstAsNoTrackingAsync(y => y.IsActive == true
        //            && y.CompanyId == sessionProvider.Session.CompanyId
        //            && y.VoucherTypeId == request.VoucherTypeId
        //            && y.Code.StartsWith(VoucherType.Code + yearMonth),
        //            OrderByDesc, null);

        //    // Extract last 4-digit serial and increment
        //    int newSerial = 1;
        //    if (lastTransaction != null)
        //    {
        //        string lastSerialStr = lastTransaction.Code.Substring((VoucherType.Code + yearMonth).Length);
        //        if (int.TryParse(lastSerialStr, out int lastSerial))
        //        {
        //            newSerial = lastSerial + 1;
        //        }
        //    }

        //    // Format the new transaction code
        //    _TransactionCode = newSerial.ToString().PadLeft(4, '0'); // 4-digit serial

        //    codes.Add($"{VoucherType.Code}{yearMonth}{_TransactionCode}");
        //    return $"{VoucherType.Code}{yearMonth}{_TransactionCode}";
        //}

    }
}
