using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Handler
{
    public class GetAllShopTypeHandler : IRequestHandler<GetAllShopTypeQuery, Tuple<IEnumerable<GetShopType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllShopTypeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetShopType>, long>> Handle(GetAllShopTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ShopType, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.ShopType, object>>[] includes = {
            };

            Expression<Func<Entities.Models.ShopType, object>> OrderBy = null;
            Expression<Func<Entities.Models.ShopType, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.ShopType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var shopType = mapper.Map<IEnumerable<GetShopType>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetShopType>, long>(shopType, entity.Item2);
        }
    }
}
