namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabOrderType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomFieldsSchema { get; set; }
        public long ServiceId { get; set; }
    }
}
