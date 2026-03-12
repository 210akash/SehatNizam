using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Query
{
    public class DeleteEmployeeBankQuery : IRequest<bool>
    {
        public DeleteEmployeeBankQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}