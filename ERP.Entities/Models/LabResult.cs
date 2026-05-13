namespace ERP.Entities.Models
{
    public class LabResult : BaseEntity
    {
        public long LabOrderId { get; set; }
        public long LabTestVariableId { get; set; }
        // Actual Result
        public string ResultValue { get; set; }
        // Snapshot Values
        public string VariableName { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
        public bool? IsAbnormal { get; set; }
        public LabOrder LabOrder { get; set; }
        public LabTestVariable LabTestVariable { get; set; }
    }
}
