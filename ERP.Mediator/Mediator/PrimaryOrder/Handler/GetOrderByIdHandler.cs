using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Entities.Command;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, GetOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

        public GetOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<GetOrder> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {

            var reportQuery = "EXEC GetStockTransaction";
            var stockTransactions = unitOfWorkDapper.Repository<StockTransactionDTO>()
                .QueryAsync<StockTransactionDTO>(reportQuery)
                .GetAwaiter().GetResult();


            Expression<Func<Entities.Models.Order, bool>> predicate = x => x.IsActive == true && x.Id == request.Id;

            Expression<Func<Entities.Models.Order, object>>[] includes = {
                x => x.OrderItems.Where(x => x.IsActive),
                x => x.Dealership,
                x => x.OrderStatus,
                x => x.OrderProcess,
                x => x.OrderAttachments,
                x => x.Dealership.Territory,
                x => x.Dealership.Territory.Area,
                x => x.Dealership.Territory.Area.Zone,
                x => x.Dealership.Territory.Area.Zone.Region,
                x => x.Dealership,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company
            };

            Expression<Func<Entities.Models.Order, object>> OrderBy = null;
            Expression<Func<Entities.Models.Order, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            List<string> thenInclude = new List<string>();
            thenInclude.Add("OrderItems.Item");
            thenInclude.Add("OrderItems.Item.ItemType");
            thenInclude.Add("OrderProcess.FromStatus");
            thenInclude.Add("OrderProcess.ToStatus");
            thenInclude.Add("OrderProcess.CreatedBy");
            thenInclude.Add("OrderItems.Item.UOM");

            var entity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, OrderBy, OrderByDesc, thenInclude, includes);
            var shopOrder = mapper.Map<IEnumerable<GetOrder>>(entity.Item1.ToList()).ToList();


            foreach (var order in shopOrder)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var matchingStock = stockTransactions.FirstOrDefault(s => s.ItemId == orderItem.ItemId);
                    if (matchingStock != null)
                    {
                        orderItem.LeftQuantity = (int?)matchingStock.StockQty;
                    }
                    else
                        orderItem.LeftQuantity = 0;
                }
            }

            return shopOrder.FirstOrDefault();
        }
    }
}
