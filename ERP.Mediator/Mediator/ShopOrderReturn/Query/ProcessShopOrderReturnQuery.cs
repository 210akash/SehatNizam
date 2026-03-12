using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Query
{
    public class ProcessShopOrderReturnQuery : IRequest<bool>
    {
        public ProcessShopOrderReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}