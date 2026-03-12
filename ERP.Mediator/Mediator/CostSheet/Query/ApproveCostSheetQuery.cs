using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Query
{
    public class ApproveCostSheetQuery : IRequest<bool>
    {
        public ApproveCostSheetQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}