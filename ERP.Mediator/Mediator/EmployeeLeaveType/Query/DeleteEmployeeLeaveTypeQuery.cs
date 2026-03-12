using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Query
{
    public class DeleteEmployeeLeaveTypeQuery : IRequest<bool>
    {
        public DeleteEmployeeLeaveTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}