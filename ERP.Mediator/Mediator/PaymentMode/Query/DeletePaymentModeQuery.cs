using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Query
{
    public class DeletePaymentModeQuery : IRequest<bool>
    {
        public DeletePaymentModeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}