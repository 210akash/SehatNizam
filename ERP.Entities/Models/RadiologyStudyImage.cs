namespace ERP.Entities.Models
{
    public class RadiologyStudyImage : BaseEntity
    {
        public long RadiologyStudyResultId { get; set; }
        public virtual RadiologyStudyResult RadiologyStudyResult { get; set; }
        public string ImageUrl { get; set; }
        public int SequenceNo { get; set; }
        public string Remarks { get; set; }
    }
}
