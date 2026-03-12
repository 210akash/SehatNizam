using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.App.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Dapper;
using System.Data;

namespace ERP.Mediator.Mediator.App.Handler
{
    public class GetDealershipStockBalanceHandler : IRequestHandler<GetDealershipStockBalanceQuery, IEnumerable<GetDealershipStockBalance>>
    {
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        public GetDealershipStockBalanceHandler(IUnitOfWorkDapper unitOfWorkDapper)
        {
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<IEnumerable<GetDealershipStockBalance>> Handle(GetDealershipStockBalanceQuery request, CancellationToken cancellationToken)
        {
                var query = "StockBalanceDistributor";
                var parameters = new DynamicParameters();
                parameters.Add("@DealerShipId", request.DealershipId, DbType.Int32);
                var regionResponse = (await unitOfWorkDapper
                    .Repository<GetDealershipStockBalance>()
                    .QueryAsync<GetDealershipStockBalance>(
                        query,
                        parameters,
                        CommandType.StoredProcedure)).ToList();
                return regionResponse;
        }
    }
}
