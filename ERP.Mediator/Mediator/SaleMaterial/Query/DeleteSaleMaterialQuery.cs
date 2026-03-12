using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Query
{
    public class DeleteSaleMaterialQuery : IRequest<bool>
    {
        public DeleteSaleMaterialQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}