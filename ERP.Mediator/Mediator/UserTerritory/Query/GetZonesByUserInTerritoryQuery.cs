using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Query
{
    public class GetZonesByUserInTerritoryQuery : IRequest<List<GetZone>>
    {
        public GetZonesByUserInTerritoryQuery(Guid UserId)
        {
            this.UserId = UserId;
        }
        public Guid? UserId { get; set; }
    }
}