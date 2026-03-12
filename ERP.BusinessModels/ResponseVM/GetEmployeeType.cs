namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal NoOfLeavesPerMonth { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
