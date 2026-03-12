using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Command
{
    public class SaveDeliveryTermsCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
    }
}
