using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Query
{
    public class DeleteCostSheetQuery : IRequest<bool>
    {
        public DeleteCostSheetQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}