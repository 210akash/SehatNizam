using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Ledger.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Ledger.Handler
{
    public class GetItemBalanceHandler : IRequestHandler<GetItemBalanceQuery, decimal>
    {
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

        public GetItemBalanceHandler(IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<decimal> Handle(GetItemBalanceQuery request, CancellationToken cancellationToken)
        {
            var reportQuery = $"GetStockTransaction @ItemId = '{request.ItemId}'";

            var result = await unitOfWorkDapper.Repository<ItemBalanceResponse>()
                .QueryAsync<ItemBalanceResponse>(reportQuery);

            return result.FirstOrDefault()?.StockQty ?? 0;
        }

        public class ItemBalanceResponse
        {
            public long ItemId { get; set; }
            public decimal StockQty { get; set; }
        }
    }
}
