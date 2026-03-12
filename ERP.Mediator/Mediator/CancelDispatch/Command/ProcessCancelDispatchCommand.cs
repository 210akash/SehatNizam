using MediatR;

namespace ERP.Mediator.Mediator.CancelDispatch.Command
{
    public class ProcessCancelDispatchCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public long StatusId { get; set; }
        public bool IsReject { get; set; }
        public string Remarks { get; set; }
    }
}