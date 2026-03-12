using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class DeleteSaleReturnQuery : IRequest<bool>
    {
        public DeleteSaleReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}