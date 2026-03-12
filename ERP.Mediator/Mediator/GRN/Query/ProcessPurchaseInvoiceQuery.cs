using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class ProcessPurchaseInvoiceQuery : IRequest<long>
    {
        public ProcessPurchaseInvoiceQuery(long Id, string Comments)
        {
            this.Id = Id;
            this.Comments = Comments;
        }

        public long Id { get; set; }
        public string Comments { get; set; }
    }
}