using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Query
{
    public class GetPriceGroupByNameQuery : IRequest<List<GetRegion>>
    {
        public GetPriceGroupByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}