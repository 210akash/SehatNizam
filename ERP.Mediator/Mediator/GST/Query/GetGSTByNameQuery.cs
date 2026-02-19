using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Query
{
    public class GetGSTByNameQuery : IRequest<List<GetGST>>
    {
        public GetGSTByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}