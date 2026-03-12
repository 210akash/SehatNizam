using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Query
{
    public class ProcessSaleMaterialQuery : IRequest<bool>
    {
        public ProcessSaleMaterialQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}