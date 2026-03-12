using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Query
{
    public class GetZoneByNameQuery : IRequest<List<GetZone>>
    {
        public GetZoneByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}