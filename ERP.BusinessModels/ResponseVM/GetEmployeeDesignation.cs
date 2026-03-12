namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeDesignation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
