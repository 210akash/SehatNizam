using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Query
{
    public class GetUOMByNameQuery : IRequest<List<GetUOM>>
    {
        public GetUOMByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}