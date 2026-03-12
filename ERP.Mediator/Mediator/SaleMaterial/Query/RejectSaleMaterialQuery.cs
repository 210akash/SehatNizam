using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Query
{
    public class RejectSaleMaterialQuery : IRequest<bool>
    {
        public RejectSaleMaterialQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}