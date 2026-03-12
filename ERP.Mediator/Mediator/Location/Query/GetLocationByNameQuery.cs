using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Query
{
    public class GetLocationByNameQuery : IRequest<List<GetLocation>>
    {
        public GetLocationByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}