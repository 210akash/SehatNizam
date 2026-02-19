namespace ERP.BusinessModels.ResponseVM
{
    public class GetStore
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public long LocationId { get; set; }
        public bool FixedAsset { get; set; }
        public GetLocation Location { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
