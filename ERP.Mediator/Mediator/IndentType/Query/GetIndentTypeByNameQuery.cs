using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Query
{
    public class GetIndentTypeByNameQuery : IRequest<List<GetIndentType>>
    {
        public GetIndentTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}