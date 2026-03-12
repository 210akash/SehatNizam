using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Query
{
    public class DeleteShopTypeQuery : IRequest<long>
    {
        public DeleteShopTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}