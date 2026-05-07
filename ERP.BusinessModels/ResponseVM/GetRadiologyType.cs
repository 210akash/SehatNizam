namespace ERP.BusinessModels.ResponseVM
{
    public class GetRadiologyType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
    }
}
