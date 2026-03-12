using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class ProcessSaleReturnQuery : IRequest<bool>
    {
        public ProcessSaleReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}