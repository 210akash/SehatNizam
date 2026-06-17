namespace ERP.BusinessModels.ResponseVM
{
    public class GetRadiologyOrder
    {
        public long Id { get; set; }
        public long? AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public long RadiologyTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public long StatusId { get; set; }
        public GetAppointmentStatus Status { get; set; }
        public bool IsActive { get; set; }
        public GetRadiologyType RadiologyType { get; set; }
        public GetRadiologyStudyResult RadiologyStudyResult { get; set; }
    }
}
