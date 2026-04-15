namespace ERP.BusinessModels.ResponseVM
{
    public class GetCandidateEvaluation
    {
        public long InterviewHistoryId { get; set; }
        public long CandidateScoringScaleId { get; set; }
        public virtual GetCandidateScoringScale CandidateScoringScale { get; set; }

        public long CandidateEvaluationCategoryId { get; set; }
        public virtual GetCandidateEvaluationCategory CandidateEvaluationCategory { get; set; }
    }
}
