using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Command
{
    public class ManagerApproveLeaveCommand : IRequest<long>
    {
        public ManagerApproveLeaveCommand(long Id, string Comments)
        {
            this.Id = Id;
            this.Comments = Comments;
        }

        public long Id { get; set; }
        public string Comments { get; set; }
    }
}