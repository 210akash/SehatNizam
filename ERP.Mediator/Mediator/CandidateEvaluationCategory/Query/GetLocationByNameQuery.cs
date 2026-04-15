using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Query
{
    public class GetCandidateEvaluationCategoryByNameQuery : IRequest<List<GetCandidateEvaluationCategory>>
    {
        public GetCandidateEvaluationCategoryByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}