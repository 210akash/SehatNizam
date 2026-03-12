namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeGrade
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
