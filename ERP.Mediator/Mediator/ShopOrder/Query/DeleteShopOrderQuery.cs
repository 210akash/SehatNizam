using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Query
{
    public class DeleteShopOrderQuery : IRequest<long>
    {
        public DeleteShopOrderQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}