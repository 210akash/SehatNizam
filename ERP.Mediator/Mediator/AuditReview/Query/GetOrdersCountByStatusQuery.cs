using System;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AuditReview.Query
{
    public class GetOrdersCountByStatusQuery : IRequest<Tuple<long, long, long, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public string Code { get; set; }
        public long DealershipId { get; set; }
        public PagingData PagingData { get; set; }
    }
}