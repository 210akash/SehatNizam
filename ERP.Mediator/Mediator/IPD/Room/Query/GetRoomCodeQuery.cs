using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Query
{
    public class GetRoomCodeQuery : IRequest<string>
    {
        public GetRoomCodeQuery(long WardId, long Id)
        {
            this.WardId = WardId;
            this.Id = Id;
        }
        public long WardId { get; set; }
        public long Id { get; set; }
    }

}