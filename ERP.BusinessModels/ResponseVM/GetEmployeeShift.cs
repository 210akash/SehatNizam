namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeShift
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
