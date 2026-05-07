namespace ERP.BusinessModels.ResponseVM
{
    public class GetRadiologyOrder
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long RadiologyTypeId { get; set; }
        public string RadiologyTypeName { get; set; }
        public string ClinicalNotes { get; set; }
        public long StatusId { get; set; }
        public bool IsActive { get; set; }
    }
}
