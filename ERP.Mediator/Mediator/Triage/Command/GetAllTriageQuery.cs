using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Triage.Query
{
    public class GetAllTriageQuery : IRequest<Tuple<IEnumerable<GetTriage>, long>>
    {
        public string BookingNo { get; set; }
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}