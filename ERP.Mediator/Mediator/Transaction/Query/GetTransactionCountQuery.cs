using System;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetTransactionCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }

        public long VoucherTypeId { get; set; }
    }
}