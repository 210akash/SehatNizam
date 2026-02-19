using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Query
{
    public class GetPurchaseDemandByIdQuery : IRequest<GetPurchaseDemand>
    {
        public GetPurchaseDemandByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}