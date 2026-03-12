using MediatR;
using System;

namespace ERP.Mediator.Mediator.UserTerritory.Command
{
    public class SaveUserTerritoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public Guid? UserId { get; set; }
        public bool IsAllTerritoryCheck { get; set; }
        public long? RegionId { get; set; }
        public long? ZoneId { get; set; }
        public long? AreaId { get; set; }
        public long? TerritoryId { get; set; }
        public long? ShopId { get; set; }
    }
}
