using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Query
{
    public class GetBedByRoomQuery : IRequest<List<GetBed>>
    {
        public GetBedByRoomQuery(long RoomId)
        {
            this.RoomId = RoomId;
        }

        public long RoomId { get; set; }
    }
}