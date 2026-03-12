using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Query
{
    public class RejectCostSheetQuery : IRequest<bool>
    {
        public RejectCostSheetQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}