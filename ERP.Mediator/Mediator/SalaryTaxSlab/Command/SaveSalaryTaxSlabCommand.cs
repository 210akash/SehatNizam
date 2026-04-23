using MediatR;
namespace ERP.Mediator.Mediator.SalaryTaxSlab.Command
{
    public class SaveSalaryTaxSlabCommand : IRequest<long>
    {
        public long Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}
