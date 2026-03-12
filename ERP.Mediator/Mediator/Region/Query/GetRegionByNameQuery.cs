using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Query
{
    public class GetRegionByNameQuery : IRequest<List<GetRegion>>
    {
        public GetRegionByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}