namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabOrder
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long LabOrderTypeId { get; set; }
        public string ClinicalNotes { get; set; }
        public GetStatus Status { get; set; }
        public long StatusId { get; set; }
        public GetLabOrderType LabOrderType { get; set; }
    }
}
