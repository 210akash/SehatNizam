using MediatR;

namespace ERP.Mediator.Mediator.Priority.Command
{
    public class SavePriorityCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
