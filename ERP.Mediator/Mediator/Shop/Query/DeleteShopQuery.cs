using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class DeleteShopQuery : IRequest<long>
    {
        public DeleteShopQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}