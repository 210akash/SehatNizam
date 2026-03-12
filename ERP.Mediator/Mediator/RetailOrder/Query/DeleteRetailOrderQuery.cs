using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Query
{
    public class DeleteRetailOrderQuery : IRequest<long>
    {
        public DeleteRetailOrderQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}