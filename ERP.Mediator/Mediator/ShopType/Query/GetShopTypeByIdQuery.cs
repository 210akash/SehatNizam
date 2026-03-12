using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Query
{
    public class GetShopTypeByIdQuery : IRequest<GetShopType>
    {
        public GetShopTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}