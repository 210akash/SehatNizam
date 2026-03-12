using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetAllShopHandler : IRequestHandler<GetAllShopQuery, Tuple<IEnumerable<GetShop>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllShopHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetShop>, long>> Handle(GetAllShopQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Shop, bool>> predicate = x => x.IsActive == true
            && (request.FDate == null || x.CreatedDate >= request.FDate.Value)
            && (request.TDate == null || x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1))
            && (request.RegionId == 0 || x.Territory.Area.Zone.RegionId == request.RegionId)
            && (request.ZoneId == 0 || x.Territory.Area.ZoneId == request.ZoneId)
            && (request.AreaId == 0 || x.Territory.AreaId == request.AreaId)
            && (request.TerritoryId == 0 || x.TerritoryId == request.TerritoryId)
            && (request.StatusId == 0 || x.StatusId == request.StatusId)
            && (request.Name == "" || x.Name.ToLower().Contains(request.Name.Trim().ToLower()))
            && (request.CreatedBy == "" || x.CreatedBy.FirstName.ToLower().Contains(request.CreatedBy.Trim().ToLower()))
            && x.Id != 143;

            Expression<Func<Entities.Models.Shop, object>>[] includes = {
                x => x.Territory,
                x => x.Scheduler,
                x => x.Attachments,
                x => x.CreatedBy,
                x => x.Status,
                x => x.VerifiedBy,
                x => x.Territory.Area,
                x => x.Territory.Area.Zone,
                x => x.Territory.Area.Zone.Region
            };

            List<string> thenInclude = new List<string>();
            thenInclude.Add("Territory.Dealership");

            Expression<Func<Entities.Models.Shop, object>> OrderBy = null;
            Expression<Func<Entities.Models.Shop, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Shop>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenInclude, includes);
            var shop = mapper.Map<IEnumerable<GetShop>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetShop>, long>(shop, entity.Item2);
        }
    }
}
