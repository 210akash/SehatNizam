using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Query
{
    public class GetCandidateEvaluationCategoryByIdQuery : IRequest<GetCandidateEvaluationCategory>
    {
        public GetCandidateEvaluationCategoryByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}