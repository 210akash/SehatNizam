using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Query
{
    public class DeleteEmployeeLeaveGroupQuery : IRequest<bool>
    {
        public DeleteEmployeeLeaveGroupQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}