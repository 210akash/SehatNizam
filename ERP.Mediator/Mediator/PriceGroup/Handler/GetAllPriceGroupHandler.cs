using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PriceGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class GetAllPriceGroupHandler : IRequestHandler<GetAllPriceGroupQuery, Tuple<IEnumerable<GetPriceGroup>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllPriceGroupHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetPriceGroup>, long>> Handle(GetAllPriceGroupQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.PriceGroup, bool>> predicate = x => x.IsActive == true && x.IsDelete == false;
            Expression<Func<Entities.Models.PriceGroup, object>> OrderByDesc = x => x.Id;

            var entity = unitOfWork.Repository<Entities.Models.PriceGroup>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, OrderByDesc, null, null);
            var priceGroup = mapper.Map<IEnumerable<GetPriceGroup>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetPriceGroup>, long>(priceGroup, entity.Item2);
        }
    }
}
