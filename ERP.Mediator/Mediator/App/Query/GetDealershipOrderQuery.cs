using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.App.Query
{
    public class GetDealershipOrderQuery : IRequest<Tuple<IEnumerable<GetDealershipOrder>, long>>
    {
        public long? OrderId { get; set; }
        public long StatusId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long DealershipId { get; set; }
        public DateTime AppDateTime { get; set; }
        public PagingData PagingData { get; set; }
    }
}