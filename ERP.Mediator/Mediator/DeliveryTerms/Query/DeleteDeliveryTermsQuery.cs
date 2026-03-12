using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Query
{
    public class DeleteDeliveryTermsQuery : IRequest<bool>
    {
        public DeleteDeliveryTermsQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}