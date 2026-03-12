using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetShopsByTerritoryPagingHandler : IRequestHandler<GetShopsByTerritoryPagingQuery, Tuple<IEnumerable<GetShopBasic>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopsByTerritoryPagingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetShopBasic>, long>> Handle(GetShopsByTerritoryPagingQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Shop, bool>> predicate = x => x.IsActive == true
              && x.TerritoryId == request.TerritoryId && (request.Param == "" || x.Name.ToLower().Contains(request.Param.ToLower()));

            Expression<Func<Entities.Models.Shop, object>> OrderBy = null;
            Expression<Func<Entities.Models.Shop, object>> OrderByDesc = x => x.CreatedDate;
            var dealershipId = unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.IsActive == true && y.TerritoryId == request.TerritoryId).Result.Id;
           
            var entity = unitOfWork.Repository<Entities.Models.Shop>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, null);
            var shop = mapper.Map<IEnumerable<GetShopBasic>>(entity.Item1.ToList()).ToList();
            foreach (var item in shop)
            {
                item.DealershipId = dealershipId;
            }
            return new Tuple<IEnumerable<GetShopBasic>, long>(shop, entity.Item2);
        }
    }
}
