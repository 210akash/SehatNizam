namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeDocumentType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
