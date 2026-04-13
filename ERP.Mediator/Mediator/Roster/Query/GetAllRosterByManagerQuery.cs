using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Query
{
    public class GetAllRosterByManagerQuery : IRequest<Tuple<IEnumerable<GetRoster>, long>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long StatusId { get; set; }

        public PagingData PagingData { get; set; }
    }
}