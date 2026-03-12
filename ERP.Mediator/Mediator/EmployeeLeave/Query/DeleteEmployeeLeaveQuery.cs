using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class DeleteEmployeeLeaveQuery : IRequest<bool>
    {
        public DeleteEmployeeLeaveQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}