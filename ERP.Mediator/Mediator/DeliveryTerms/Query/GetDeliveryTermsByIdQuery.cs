using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Query
{
    public class GetDeliveryTermsByIdQuery : IRequest<GetDeliveryTerms>
    {
        public GetDeliveryTermsByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}