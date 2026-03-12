using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetShopByIdQuery : IRequest<GetShop>
    {
        public GetShopByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}