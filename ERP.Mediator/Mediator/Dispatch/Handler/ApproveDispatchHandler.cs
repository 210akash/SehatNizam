using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class ApproveDispatchHandler : IRequestHandler<ApproveDispatchQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;
        private readonly List<string> codes = new();
        public ApproveDispatchHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;
        }

        public async Task<Tuple<long, string>> Handle(ApproveDispatchQuery request, CancellationToken cancellationToken)
        {
            int check = 0;
            decimal totalPrice = 0;

            var Dispatch = await unitOfWork
                .Repository<Entities.Models.Dispatch>()
                .GetFirstAsync(
                  y => y.Id == request.Id && y.StatusId == 2,
                    null, null,
                    "DispatchOrder," +
                    "DispatchOrder.Order," +
                    "DispatchOrder.DispatchDetail," +
                    "DispatchOrder.DispatchDetail.CostSheet," +
                    "DispatchOrder.DispatchDetail.CostSheet.CostSheetDetail," +
                    "DispatchOrder.DispatchDetail.OrderItem");
                
            if (Dispatch != null)
            {
                #region Get Transactions Accounts
                var Accountsdepartment = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Accounts");
                if (Accountsdepartment == null)
                {
                    return new Tuple<long, string>(502, "Accounts Department not found!");
                }

                var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(x => x.Name == "Sale");
                if (department == null)
                {
                    return new Tuple<long, string>(502, "Department not found!");
                }

                var ProductionCostaccount = await unitOfWork.Repository<Entities.Models.Account>()
                    .GetFirstAsNoTrackingAsync(x => x.Code == "0501010001");
                if (ProductionCostaccount == null)
                {
                    return new Tuple<long, string>(503, "COGS Production Cost Account not found!");
                }

                var finnishStoreaccount = await unitOfWork.Repository<Entities.Models.Account>()
                    .GetFirstAsNoTrackingAsync(x => x.Code == "0102120003");
                if (finnishStoreaccount == null)
                {
                    return new Tuple<long, string>(503, "Finished Goods Store Account not found!");
                }

                var Freightaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0503090001");
                if (Freightaccount == null)
                {
                    return new Tuple<long, string>(503, "Freight Account not found!");
                }

           

                var account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0401010001");
                if (account == null)
                {
                    return new Tuple<long, string>(503, "Distributors Sale Account not found!");
                }

                var tradepromoaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0401060003");
                if (tradepromoaccount == null)
                {
                    return new Tuple<long, string>(503, "Trade Promo Account not found!");
                }

                var trademarginaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0401080002");
                if (trademarginaccount == null)
                {
                    return new Tuple<long, string>(503, "Trade Margin Account not found!");
                }

                var distributormarginaccount = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.Code == "0401070001");
                if (distributormarginaccount == null)
                {
                    return new Tuple<long, string>(503, "Distributor Margin Account not found!");
                }

                #endregion

                foreach (var item in Dispatch.DispatchOrder.Where(y=>y.IsActive))
                {
                    totalPrice = 0;
                    foreach (var detail in item.DispatchDetail.Where(y => y.IsActive))
                    {
                        decimal itemTotalPrice = (decimal)(detail.OrderItem.RetailPrice * detail.Quantity);
                        totalPrice += itemTotalPrice;
                    }

                    #region Save Vouchers For Sale And Freight

                    #region Sale Voucher

                    var accountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.DealershipId == item.Order.DealershipId);
                    if (accountGroup == null)
                    {
                        var dealer = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == item.Order.DealershipId);
                        return new Tuple<long, string>(501, "GL Account not found against " + dealer.Name + "!");
                    }

                    SaveTransactionCommand tCommand = new()
                    {
                        Date = Dispatch.CreatedDate.Value,
                        ReferenceNumber = item.DCCode,
                        Remarks = "Amount deducted from distributor against challan no " + item.DCCode,
                        VoucherTypeId = 6,
                        StatusId = 3,
                        OrderId = item.OrderId
                    };

                    // 1- Distributor Account Debit
                    List<SaveTransactionDetailCommand> tDCommandList = new();
                    SaveTransactionDetailCommand tDDCommand = new()
                    {
                        AccountGroupId = accountGroup.Id,
                        DepartmentId = department.Id,
                        ProjectId = 3,
                        IsGroup = true,
                        DebitAmount = item.DistributorAmount,
                        CreditAmount = 0,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity)
                    };
                    tDCommandList.Add(tDDCommand);


                    // 2- Trade Promo
                    SaveTransactionDetailCommand tTMcommand = new()
                    {
                        AccountId = tradepromoaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = item.TradePromo,
                        CreditAmount = 0,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity)
                    };
                    tDCommandList.Add(tTMcommand);

                    // 3- Trade Margin
                    SaveTransactionDetailCommand tTPcommand = new()
                    {
                        AccountId = trademarginaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = item.TradeMargin,
                        CreditAmount = 0,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity)
                    };
                    tDCommandList.Add(tTPcommand);

                    // 4- Distributor Margin
                    SaveTransactionDetailCommand tDMcommand = new()
                    {
                        AccountId = distributormarginaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = item.DistributorMargin,
                        CreditAmount = 0,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity)
                    };
                    tDCommandList.Add(tDMcommand);

                    // 5- Distributor Sale Credit
                    SaveTransactionDetailCommand tDCCommand = new()
                    {
                        AccountId = account.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        DebitAmount = 0,
                        CreditAmount = totalPrice,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity)
                    };
                    tDCommandList.Add(tDCCommand);

                    tCommand.TransactionDetails = tDCommandList;
                    await SaveTransaction(tCommand);
                    #endregion

                    #region Freight Charges Voucher
                    
                    SaveTransactionCommand tFreightCommand = new()
                    {
                        Date = Dispatch.CreatedDate.Value,
                        ReferenceNumber = item.DCCode,
                        Remarks = "Freight charges against challan no " + item.DCCode,
                        VoucherTypeId = 1,
                        StatusId = 3,
                        OrderId = item.OrderId
                    };

                    List<SaveTransactionDetailCommand> tFreightDCommandList = new();
                    SaveTransactionDetailCommand tFreightDDCommand = new()
                    {
                        AccountGroupId = accountGroup.Id,
                        DepartmentId = department.Id,
                        ProjectId = 3,
                        IsGroup = true,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity),
                        DebitAmount = 0,
                        CreditAmount = item.OrderFreightCharges
                    };
                    tFreightDCommandList.Add(tFreightDDCommand);

                    SaveTransactionDetailCommand tFreightDCCommand = new()
                    {
                        AccountId = Freightaccount.Id,
                        DepartmentId = Accountsdepartment.Id,
                        ProjectId = 3,
                        IsGroup = false,
                        Quantity = item.DispatchDetail.Where(y => y.IsActive).Sum(y => y.Quantity),
                        DebitAmount = item.OrderFreightCharges,
                        CreditAmount = 0
                    };
                    tFreightDCommandList.Add(tFreightDCCommand);
                    tFreightCommand.TransactionDetails = tFreightDCommandList;
                    await SaveTransaction(tFreightCommand);

                    #endregion

                    #endregion

                  //  Update DispatchOrder Status
                   var updateDispatchOrder = unitOfWork.Repository<Entities.Models.DispatchOrder>()
                       .GetFirst(y => y.Id == item.Id, includeProperties: null);

                    if (updateDispatchOrder != null)
                    {
                        // Just update the fields you want
                        updateDispatchOrder.StatusId = (long?)OrderStatusEnum.OrderDispatched;
                        updateDispatchOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updateDispatchOrder.ModifiedDate = DateTime.Now;

                        // Do NOT touch updateDispatchOrder.DispatchDetail at all
                        unitOfWork.Repository<Entities.Models.DispatchOrder>().Update(updateDispatchOrder);
                    }

                    #region Update Order Status And History
                    var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsync(y => y.Id == item.OrderId);
                    Entities.Models.OrderProcess process = new()
                    {
                        OrderId = item.OrderId,
                        FromStatusId = order.OrderStatusId,
                        ToStatusId = (long)OrderStatusEnum.OrderDispatched,
                        Comments = "Order Dispatched against DC " + item.DCCode,
                        TransactionId = item.DCCode,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now
                    };
                    unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
                    order.OrderStatusId = (long)OrderStatusEnum.OrderDispatched;
                    unitOfWork.Repository<Entities.Models.Order>().Update(order);
                    #endregion

                }

                #region Material Voucher

                decimal totalCOGSProductionCost = 0;
                decimal totalQuantity = 0;

                foreach (var Orderitem in Dispatch.DispatchOrder.Where(y => y.IsActive))
                {
                    foreach (var item in Orderitem.DispatchDetail.Where(y => y.IsActive))
                    {
                        decimal costPerPet = item.CostSheet.CostPerPet;
                        decimal itemCOGSProductionCost = costPerPet * item.Quantity;
                        totalCOGSProductionCost += itemCOGSProductionCost;
                        totalQuantity += item.Quantity;
                    }
                }

                var orderRefs = string.Join(", ", Dispatch.DispatchOrder.Where(y => y.IsActive).Select(x => x.Order.Id)); // Assuming OrderCode is available

                // Create one transaction for all dispatch items
                SaveTransactionCommand tMaterialCommand = new()
                {
                    Date = Dispatch.CreatedDate.Value,
                    ReferenceNumber = Dispatch.Code,
                    Remarks = $"Production Cost charge for Dispatch No. {Dispatch.Code}, Orders: {orderRefs}",
                    VoucherTypeId = 1,
                    StatusId = 3
                };

                List<SaveTransactionDetailCommand> tMaterialDCommandList = new()
                    {
                        // 1- COGS Production Cost (Debit)
                        new SaveTransactionDetailCommand
                        {
                            AccountId = ProductionCostaccount.Id,
                            DepartmentId = department.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            Quantity = totalQuantity,
                            DebitAmount = totalCOGSProductionCost,
                            CreditAmount = 0
                        },

                        // 2- Finished Goods Store (Credit)
                        new SaveTransactionDetailCommand
                        {
                            AccountId = finnishStoreaccount.Id,
                            DepartmentId = Accountsdepartment.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            Quantity = totalQuantity,
                            DebitAmount = 0,
                            CreditAmount = totalCOGSProductionCost
                        }
                    };

                tMaterialCommand.TransactionDetails = tMaterialDCommandList;
                await SaveTransaction(tMaterialCommand);

                #endregion

                var updateDispatch = unitOfWork.Repository<Entities.Models.Dispatch>().GetFirst(y => y.Id == Dispatch.Id);
                updateDispatch.StatusId = 3;
                updateDispatch.ApprovedDate = DateTime.Now;
                updateDispatch.ApprovedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Dispatch>().Update(updateDispatch);
                check = await unitOfWork.SaveChangesAsync();
            }

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Dispatch Approved Successful!");
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

        //    return $"{VoucherType.Code}{yearMonth}{_TransactionCode}";
        //}
    }
}
