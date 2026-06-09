using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Query
{
    public class GetBedByRoomQuery : IRequest<List<GetBed>>
    {
        public GetBedByRoomQuery(long RoomId, bool Vacant)
        {
            this.RoomId = RoomId;
            this.Vacant = Vacant;
        }

        public long RoomId { get; set; }
        public bool Vacant { get; set; }
    }
}