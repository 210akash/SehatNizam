using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Command
{
    public class RejectEmployeeLeaveCommand : IRequest<long>
    {
        public RejectEmployeeLeaveCommand(long Id, string Comments)
        {
            this.Id = Id;
            this.Comments = Comments;
        }

        public long Id { get; set; }
        public string Comments { get; set; }
    }
}