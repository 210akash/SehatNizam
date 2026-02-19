using MediatR;

namespace ERP.Mediator.Mediator.Location.Command
{
    public class SaveLocationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
