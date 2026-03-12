using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Query
{
    public class GetInterviewByNameQuery : IRequest<List<GetInterview>>
    {
        public GetInterviewByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}