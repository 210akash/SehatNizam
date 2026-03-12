using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Command
{
    public class ProcessEmployeeLeaveCommand : IRequest<long>
    {
        public ProcessEmployeeLeaveCommand(long Id, string Comments)
        {
            this.Id = Id;
            this.Comments = Comments;
        }

        public long Id { get; set; }
        public string Comments { get; set; }
    }
}