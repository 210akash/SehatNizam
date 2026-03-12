using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetSalesTargetByZoneIdHandler;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class GetSalesTargetByZoneIdHandler : IRequestHandler<GetSalesTargetByZoneIdQuery, List<GroupedSalesTarget>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetSalesTargetByZoneIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<List<GroupedSalesTarget>> Handle(GetSalesTargetByZoneIdQuery request, CancellationToken cancellationToken)
        {
            var salesTarget = await unitOfWork.Repository<Entities.Models.SalesTarget>()
                .GetAsync(y => y.TargetMonth.Month == request.TargetMonth.Month,
                          null, null, "User,User.UserTerritory,User.UserTerritory.Zone,User.UserTerritory.Region,User.UserTerritory.Territory,User.UserTerritory.Area");

            var _salesTarget = mapper.Map<List<GetSalesTarget>>(salesTarget);

            // First, map and extract the relevant data (active UserTerritory)
            var processedItems = _salesTarget
                .Select(item =>
                {
                    var activeTerritory = item.User.UserTerritory.FirstOrDefault(x => x.IsActive);
                    return new
                    {
                        Item = item,
                        ActiveTerritory = activeTerritory
                    };
                })
                .Where(x => x.ActiveTerritory != null)
                .ToList();

            // Now, group by region, zone, area, and territory
            var groupedItems = processedItems
                .GroupBy(x => x.ActiveTerritory.Region.Name)
                .Select(region => new GroupedSalesTarget
                {
                    Region = region.Key,
                    Target = region.Sum(x => x.Item.Target),
                    Zones = region
                        .Where(x => x.ActiveTerritory.ZoneId != null)
                        .GroupBy(x => x.ActiveTerritory.Zone?.Name)
                        .Select(zone => new ZoneGroup
                        {
                            Zone = zone.Key,
                            Target = zone.Sum(x => x.Item.Target),
                            Areas = zone
                                .Where(x => x.ActiveTerritory.AreaId != null)
                                .GroupBy(x => x.ActiveTerritory.Area?.Name)
                                .Select(area => new AreaGroup
                                {
                                    Area = area.Key,
                                    Target = area.Sum(x => x.Item.Target),
                                    Territories = area
                                        .Where(x => x.Item.UserId != null)
                                        .Select(x => new TerritoryGroup
                                        {
                                            Territory = x.Item.User.UserTerritory.FirstOrDefault(y => y.IsActive)?.Territory.Name,
                                            Target = x.Item.Target,
                                            Items = new List<GetSalesTarget> { x.Item }
                                        })
                                })
                        })
                })
                .ToList();

            return groupedItems;
        }

        public class GroupedSalesTarget
        {
            public string Region { get; set; }
            public decimal Target { get; set; }
            public IEnumerable<ZoneGroup> Zones { get; set; }
        }

        public class ZoneGroup
        {
            public string Zone { get; set; }
            public decimal Target { get; set; }
            public IEnumerable<AreaGroup> Areas { get; set; }
        }

        public class AreaGroup
        {
            public string Area { get; set; }
            public decimal Target { get; set; }
            public IEnumerable<TerritoryGroup> Territories { get; set; }
        }

        public class TerritoryGroup
        {
            public string Territory { get; set; }
            public decimal Target { get; set; }
            public IEnumerable<GetSalesTarget> Items { get; set; }
        }
    }
}
