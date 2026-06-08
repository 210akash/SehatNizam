using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Query
{
    public class GetBedCodeQuery : IRequest<string>
    {
        public GetBedCodeQuery(long RoomId,long Id)
        {
            this.RoomId = RoomId;
            this.Id = Id;
        }
        public long RoomId { get; set; }
        public long Id { get; set; }
    }

}