namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeLeaveType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
