using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Query
{
    public class GetAllRoomQuery : IRequest<Tuple<IEnumerable<GetRoom>, long>>
    {
        public long? WardId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public PagingData PagingData { get; set; }
    }
}