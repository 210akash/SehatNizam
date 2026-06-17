namespace ERP.BusinessModels.ResponseVM
{
    public class GetRadiologyStudyImage
    {
        public long Id { get; set; }
        public long RadiologyStudyResultId { get; set; }
        public string ImageUrl { get; set; }
        public int SequenceNo { get; set; }
        public string Remarks { get; set; }
    }
}
