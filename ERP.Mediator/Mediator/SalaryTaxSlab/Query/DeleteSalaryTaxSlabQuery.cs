using MediatR;

namespace ERP.Mediator.Mediator.SalaryTaxSlab.Query
{
    public class DeleteSalaryTaxSlabQuery : IRequest<bool>
    {
        public DeleteSalaryTaxSlabQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}