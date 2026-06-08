using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Query
{
    public class GetAllBedQuery : IRequest<Tuple<IEnumerable<GetBed>, long>>
    {
        public long? RoomId { get; set; }
        public long? CompanyId { get; set; }
        public string BedNo { get; set; }

        public PagingData PagingData { get; set; }
    }
}