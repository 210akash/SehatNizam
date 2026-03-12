using MediatR;

namespace ERP.Mediator.Mediator.RejectReason.Command
{
    public class SaveRejectReasonCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
