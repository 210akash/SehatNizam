namespace ERP.Entities.Models
{
    public class LabResult : BaseEntity
    {
        public long LabOrderId { get; set; }
        public long LabTestVariableId { get; set; }
        public string ResultValue { get; set; }
        public LabOrder LabOrder { get; set; }
        public LabTestVariable LabTestVariable { get; set; }
    }
}
