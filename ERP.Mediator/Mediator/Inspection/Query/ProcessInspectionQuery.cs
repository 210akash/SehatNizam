using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class ProcessInspectionQuery : IRequest<bool>
    {
        public ProcessInspectionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}