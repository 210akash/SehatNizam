using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Query
{
    public class GetPurchaseOrderByIdQuery : IRequest<GetPurchaseOrder>
    {
        public GetPurchaseOrderByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}