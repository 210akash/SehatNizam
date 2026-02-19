using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Query
{
    public class GetPriorityByNameQuery : IRequest<List<GetPriority>>
    {
        public GetPriorityByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}