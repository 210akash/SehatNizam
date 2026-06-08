using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Command
{
    public class SaveBedCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long RoomId { get; set; }
        public string Code { get; set; }
        public string BedNo { get; set; }
        public string Description { get; set; }
    }
}
