using MediatR;

namespace ERP.Mediator.Mediator.VisitType.Command
{
    public class SaveVisitTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
