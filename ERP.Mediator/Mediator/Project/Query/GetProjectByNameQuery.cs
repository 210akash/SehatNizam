using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Query
{
    public class GetProjectByNameQuery : IRequest<List<GetProject>>
    {
        public GetProjectByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}