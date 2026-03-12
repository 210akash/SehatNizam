using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class GetShopOrderByIdQuery : IRequest<GetShopOrder>
    {
        public GetShopOrderByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}