using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Query
{
    public class DeleteShopOrderReturnQuery : IRequest<bool>
    {
        public DeleteShopOrderReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}