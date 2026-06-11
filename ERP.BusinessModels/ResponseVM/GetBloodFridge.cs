namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodFridge
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
