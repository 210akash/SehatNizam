using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class ApproveInspectionQuery : IRequest<bool>
    {
        public ApproveInspectionQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}