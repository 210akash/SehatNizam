using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dispatch.Query
{
    public class GetOrdersToDispatchQuery : IRequest<Tuple<IEnumerable<GetOrder>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long? DealershipId { get; set; }
        public string Code { get; set; }
        public PagingData PagingData { get; set; }
    }
}