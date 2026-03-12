using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Command
{
    public class SaveEmployeeTypeCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal NoOfLeavesPerMonth { get; set; }
    }
}
