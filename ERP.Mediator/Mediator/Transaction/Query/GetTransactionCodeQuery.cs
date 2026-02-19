using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetTransactionCodeQuery : IRequest<string>
    {
        public GetTransactionCodeQuery(long VoucherTypeId) { 
        this.VoucherTypeId = VoucherTypeId;
        }

        public long VoucherTypeId { get; set; }
    }
}