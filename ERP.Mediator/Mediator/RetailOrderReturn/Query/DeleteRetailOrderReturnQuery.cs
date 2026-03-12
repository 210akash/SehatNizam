using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Query
{
    public class DeleteRetailOrderReturnQuery : IRequest<bool>
    {
        public DeleteRetailOrderReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}