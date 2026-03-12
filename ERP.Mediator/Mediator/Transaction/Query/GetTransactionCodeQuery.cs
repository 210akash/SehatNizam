using MediatR;
using System;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetTransactionCodeQuery : IRequest<string>
    {
        public GetTransactionCodeQuery(long VoucherTypeId, DateTime VoucherDate) { 
               this.VoucherTypeId = VoucherTypeId;
               this.VoucherDate = VoucherDate;
        }

        public long VoucherTypeId { get; set; }
        public DateTime VoucherDate { get; set; }
    }
}