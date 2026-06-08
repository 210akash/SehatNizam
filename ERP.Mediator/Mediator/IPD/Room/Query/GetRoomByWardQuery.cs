using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Query
{
    public class GetRoomByWardQuery : IRequest<List<GetRoom>>
    {
        public GetRoomByWardQuery(long WardId)
        {
            this.WardId = WardId;
        }

        public long WardId { get; set; }
    }
}