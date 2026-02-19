using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Query
{
    public class GetPaymentModeByIdQuery : IRequest<GetPaymentMode>
    {
        public GetPaymentModeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}