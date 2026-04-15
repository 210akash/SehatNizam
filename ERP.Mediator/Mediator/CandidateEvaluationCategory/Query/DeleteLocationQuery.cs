using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Query
{
    public class DeleteCandidateEvaluationCategoryQuery : IRequest<bool>
    {
        public DeleteCandidateEvaluationCategoryQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}