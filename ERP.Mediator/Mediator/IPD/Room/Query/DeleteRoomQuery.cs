using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Query
{
    public class DeleteRoomQuery : IRequest<bool>
    {
        public DeleteRoomQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}