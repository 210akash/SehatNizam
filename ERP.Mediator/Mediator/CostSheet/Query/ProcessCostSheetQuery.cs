using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Query
{
    public class ProcessCostSheetQuery : IRequest<bool>
    {
        public ProcessCostSheetQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}