namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodComponentType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ShelfLifeDays { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
