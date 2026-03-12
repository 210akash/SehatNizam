namespace ERP.BusinessModels.ResponseVM
{
    public class GetProjectStore
    {
        public long ProjectId { get; set; }
        public GetProject Project { get; set; }

        public long StoreId { get; set; }
        public GetStore Store { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
