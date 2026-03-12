using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class DeleteSaleMaterialReturnQuery : IRequest<bool>
    {
        public DeleteSaleMaterialReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}