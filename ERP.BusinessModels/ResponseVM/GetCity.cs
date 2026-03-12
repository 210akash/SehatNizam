namespace ERP.BusinessModels.ResponseVM
{
    public class GetCity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
