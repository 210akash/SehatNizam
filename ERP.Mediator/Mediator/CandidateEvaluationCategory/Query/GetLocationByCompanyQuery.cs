using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Query
{
    public class GetCandidateEvaluationCategoryByCompanyQuery : IRequest<List<GetCandidateEvaluationCategory>>
    {
        public GetCandidateEvaluationCategoryByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}