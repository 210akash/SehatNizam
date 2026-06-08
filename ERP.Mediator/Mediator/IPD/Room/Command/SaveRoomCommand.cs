using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Command
{
    public class SaveRoomCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long WardId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
