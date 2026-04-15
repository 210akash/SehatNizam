using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

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
        public virtual List<SaveCandidateEvaluationCommand> CandidateEvaluations { get; set; }
    }

    public class SaveCandidateEvaluationCommand
    {
        public long InterviewHistoryId { get; set; }
        public long CandidateScoringScaleId { get; set; }
        public long CandidateEvaluationCategoryId { get; set; }
    }

}
