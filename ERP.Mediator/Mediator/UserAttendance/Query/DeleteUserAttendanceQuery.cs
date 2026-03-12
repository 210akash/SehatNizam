using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class DeleteUserAttendanceQuery : IRequest<long>
    {
        public DeleteUserAttendanceQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}