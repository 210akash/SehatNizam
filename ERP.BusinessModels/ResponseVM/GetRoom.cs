namespace ERP.BusinessModels.ResponseVM
{
    public class GetRoom
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long WardId { get; set; }
        public GetWard Ward { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
