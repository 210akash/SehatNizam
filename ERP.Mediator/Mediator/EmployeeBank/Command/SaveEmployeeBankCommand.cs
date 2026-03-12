using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Command
{
    public class SaveEmployeeBankCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }
}
