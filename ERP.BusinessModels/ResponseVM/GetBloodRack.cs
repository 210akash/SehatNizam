namespace ERP.BusinessModels.ResponseVM
{
    public class GetBloodRack
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long BloodFridgeId { get; set; }
        public GetBloodFridge BloodFridge { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
