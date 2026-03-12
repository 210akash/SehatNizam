using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Mediator.Mediator.Transaction.Command;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class ApproveSaleReturnHandler : IRequestHandler<ApproveSaleReturnQuery, Tuple<long, string>> 
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMapper mapper;

        public ApproveSaleReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mapper = mapper;   
        }

        public async Task<Tuple<long, string>> Handle(ApproveSaleReturnQuery request, CancellationToken cancellationToken)
        {
            var SaleReturn = await unitOfWork
             .Repository<Entities.Models.SaleReturn>()
             .GetFirstAsync(
               y => y.Id == request.Id,
                 null, null,
                 "DispatchOrder," +
                 "DispatchOrder.Order," +
                 "SaleReturnDetail.DispatchDetail," +
                 "DispatchOrder.DispatchDetail.CostSheet");

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

            #region Sale Voucher

            decimal DistributorAmount = 0;
            decimal TradePromo = 0;
            decimal TradeMargin = 0;
            decimal DistributorMargin = 0;

            // 3. Loop through the details
            foreach (var detail in SaleReturn.SaleReturnDetail)
            {
                var orderItem = await unitOfWork.Repository<OrderItems>()
                    .GetFirstAsync(o => o.Id == detail.DispatchDetail.OrderItemId);

                DistributorAmount += orderItem.DistributorPrice * detail.Quantity;
                TradePromo += orderItem.DistributorPromo.GetValueOrDefault() * detail.Quantity;
                TradeMargin += (orderItem.RetailPrice.GetValueOrDefault()
                                       - orderItem.TradePrice
                                       - orderItem.DistributorPromo.GetValueOrDefault()) * detail.Quantity;
                DistributorMargin += (orderItem.TradePrice - orderItem.DistributorPrice) * detail.Quantity;
            }


            var accountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(x => x.DealershipId == SaleReturn.DispatchOrder.Order.DealershipId);
            if (accountGroup == null)
            {
                var dealer = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == SaleReturn.DispatchOrder.Order.DealershipId);
                return new Tuple<long, string>(501, "GL Account not found against " + dealer.Name + "!");
            }

            decimal totalQty = SaleReturn.SaleReturnDetail.Where(y => y.IsActive).Sum(y => y.Quantity);

            SaveTransactionCommand tCommand = new()
            {
                Date = SaleReturn.CreatedDate.Value,
                ReferenceNumber = SaleReturn.DispatchOrder.DCCode,
                Remarks = "Booked Sales Returned from distributor against challan no " + SaleReturn.DispatchOrder.DCCode,
                VoucherTypeId = 9,
                StatusId = 3,
                OrderId = SaleReturn.DispatchOrder.OrderId
            };

            // 1- Distributor Account Debit
            List<SaveTransactionDetailCommand> tDCommandList = new();
            SaveTransactionDetailCommand tDDCommand = new()
            {
                AccountGroupId = accountGroup.Id,
                DepartmentId = department.Id,
                ProjectId = 3,
                IsGroup = true,
                DebitAmount = 0,
                CreditAmount = DistributorAmount,
                Quantity = totalQty
            };
            tDCommandList.Add(tDDCommand);


            // 2- Trade Promo
            SaveTransactionDetailCommand tTMcommand = new()
            {
                AccountId = tradepromoaccount.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = false,
                DebitAmount = 0,
                CreditAmount = TradePromo,
                Quantity = totalQty
            };
            tDCommandList.Add(tTMcommand);

            // 3- Trade Margin
            SaveTransactionDetailCommand tTPcommand = new()
            {
                AccountId = trademarginaccount.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = false,
                DebitAmount = 0,
                CreditAmount = TradeMargin,
                Quantity = totalQty
            };
            tDCommandList.Add(tTPcommand);

            // 4- Distributor Margin
            SaveTransactionDetailCommand tDMcommand = new()
            {
                AccountId = distributormarginaccount.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = false,
                DebitAmount = 0,
                CreditAmount = DistributorMargin,
                Quantity = totalQty
            };
            tDCommandList.Add(tDMcommand);

            // 5- Distributor Sale Debit
            SaveTransactionDetailCommand tDCCommand = new()
            {
                AccountId = account.Id,
                DepartmentId = Accountsdepartment.Id,
                ProjectId = 3,
                IsGroup = false,
                DebitAmount = DistributorAmount + TradePromo + TradeMargin + DistributorMargin,
                CreditAmount = 0,
                Quantity = totalQty
            };

            tDCommandList.Add(tDCCommand);
            tCommand.TransactionDetails = tDCommandList;
            await SaveTransaction(tCommand);
            #endregion

            #region Material Voucher

            decimal totalCOGSProductionCost = 0;
            decimal totalQuantity = 0;

            foreach (var SRitem in SaleReturn.SaleReturnDetail.Where(y => y.IsActive))
            {
                decimal costPerPet = SRitem.DispatchDetail.CostSheet.CostPerPet;
                decimal itemCOGSProductionCost = costPerPet * SRitem.Quantity;
                totalCOGSProductionCost += itemCOGSProductionCost;
                totalQuantity += SRitem.Quantity;
            }

            // Create one transaction for all dispatch items
            SaveTransactionCommand tMaterialCommand = new()
            {
                Date = SaleReturn.CreatedDate.Value,
                ReferenceNumber = SaleReturn.Code,
                Remarks = $"Production Cost reversal for Stock Returned Note No. {SaleReturn.Code}  against Dispatch No. {SaleReturn.DispatchOrder.DCCode}, Order: {SaleReturn.DispatchOrder.Order.Id}",
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
                            DebitAmount = 0,
                            CreditAmount = totalCOGSProductionCost
                        },

                        // 2- Finished Goods Store (Credit)
                        new SaveTransactionDetailCommand
                        {
                            AccountId = finnishStoreaccount.Id,
                            DepartmentId = Accountsdepartment.Id,
                            ProjectId = 3,
                            IsGroup = false,
                            Quantity = totalQuantity,
                            DebitAmount = totalCOGSProductionCost,
                            CreditAmount = 0
                        }
                    };

            tMaterialCommand.TransactionDetails = tMaterialDCommandList;
            await SaveTransaction(tMaterialCommand);

            #endregion

            SaleReturn.StatusId = 3;
            SaleReturn.ApprovedDate = DateTime.Now;
            SaleReturn.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SaleReturn>().Update(SaleReturn);
            var check = await unitOfWork.SaveChangesAsync();
            if (check > 0)
            {
                return new Tuple<long, string>(200, "Sale Return Approved Successful!");
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
