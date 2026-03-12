using System;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class GetSaleReturnCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public long DealershipId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}