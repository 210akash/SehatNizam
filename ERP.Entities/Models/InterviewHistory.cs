using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class InterviewHistory : BaseEntity
    {
        public long InterviewId { get; set; }
        public virtual Interview Interview { get; set; }
        public DateTime? InterviewDate { get; set; }
        public int? JoinAfterDays { get; set; }
        public string Comments { get; set; }
        public long StatusId { get; set; }
        public virtual Status Status { get; set; }
        public virtual List<InterviewAttendees> InterviewAttendees { get; set; }
        public virtual List<CandidateEvaluation> CandidateEvaluations { get; set; }
    }

    public class CandidateEvaluation : BaseEntity
    {
        public long InterviewHistoryId { get; set; }
        public virtual InterviewHistory InterviewHistory { get; set; }

        public long CandidateScoringScaleId { get; set; }
        public virtual CandidateScoringScale CandidateScoringScale { get; set; }

        public long CandidateEvaluationCategoryId { get; set; }
        public virtual CandidateEvaluationCategory CandidateEvaluationCategory { get; set; }
    }

    public class InterviewAttendees : BaseEntity
    {
        public long InterviewHistoryId { get; set; }
        public virtual InterviewHistory InterviewHistory { get; set; }
        public Guid AspNetUsersId { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
