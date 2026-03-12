using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Query
{
    public class IsZoneTargetExistQuery : IRequest<bool>
    {
        public IsZoneTargetExistQuery(long ZoneId)
        {
            this.ZoneId = ZoneId;
        }

        public long ZoneId { get; set; }
    }
}