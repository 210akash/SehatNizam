using MediatR;
using System;

namespace ERP.Mediator.Mediator.Interview.Command
{
    public class AddCommentsCommand : IRequest<long>
    {
        public long InterviewId { get; set; }
        public long StatusId { get; set; }
        public DateTime? InterviewDate { get; set; }
        public int? JoinAfterDays { get; set; }
        public string Comments { get; set; }
        public string[] InterviewAttendees { get; set; }
    }
}
