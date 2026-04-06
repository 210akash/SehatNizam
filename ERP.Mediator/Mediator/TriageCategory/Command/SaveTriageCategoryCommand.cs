using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Command
{
    public class SaveTriageCategoryCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
