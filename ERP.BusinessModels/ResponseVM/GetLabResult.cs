namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabResult
    {
        public long Id { get; set; }
        public long LabOrderId { get; set; }
        public string ResultValue { get; set; }
        public string VariableName { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
        public bool? IsAbnormal { get; set; }
    }
}
