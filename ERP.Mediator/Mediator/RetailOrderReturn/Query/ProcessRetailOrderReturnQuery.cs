using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Query
{
    public class ProcessRetailOrderReturnQuery : IRequest<bool>
    {
        public ProcessRetailOrderReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}