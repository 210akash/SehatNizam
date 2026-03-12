using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class GetFieldMapFilterEFQuery : IRequest<GetFieldMapFilterEF> 
    {
        public int? RegionId { get; set; }
        public int? ZoneId { get; set; }
        public int? AreaId { get; set; }
        public int? TerritoryId { get; set; }

        public int? DealershipEnabled { get; set; }
        public int? DealershipId { get; set; }

        public int? ShopEnabled { get; set; }
        public int? ShopId { get; set; }
    }
}
