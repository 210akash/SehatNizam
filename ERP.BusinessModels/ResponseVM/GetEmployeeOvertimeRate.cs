namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeOvertimeRate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
