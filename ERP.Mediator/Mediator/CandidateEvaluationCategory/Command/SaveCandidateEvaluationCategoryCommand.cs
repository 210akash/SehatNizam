using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Command
{
    public class SaveCandidateEvaluationCategoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
