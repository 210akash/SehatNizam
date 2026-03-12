using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class ProcessSaleMaterialReturnQuery : IRequest<bool>
    {
        public ProcessSaleMaterialReturnQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}