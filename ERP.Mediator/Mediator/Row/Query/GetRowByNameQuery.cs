using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Row.Query
{
    public class GetRowByNameQuery : IRequest<List<GetRow>>
    {
        public GetRowByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}