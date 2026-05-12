namespace ERP.Entities.Models
{
    public class RadiologyOrder : BaseEntity
    {
        public long? AppointmentId { get; set; }
        public long RadiologyTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public long StatusId { get; set; }
        public Status Status { get; set; }
        public Appointment Appointment { get; set; }
        public RadiologyType RadiologyType { get; set; }
    }

    //public class RadiologyResult : BaseEntity
    //{
    //    public long RadiologyOrderId { get; set; }
    //    public string Findings { get; set; }
    //    public string Impression { get; set; }
    //    public string Recommendations { get; set; }
    //    public string DoctorNotes { get; set; }
    //    public JsonDocument? StructuredData { get; set; }
    //    public string ReportFileUrl { get; set; }
    //    public RadiologyOrder RadiologyOrder { get; set; }
    //}
}
