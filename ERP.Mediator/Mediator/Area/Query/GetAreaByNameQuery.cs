using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Query
{
    public class GetAreaByNameQuery : IRequest<List<GetArea>>
    {
        public GetAreaByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}