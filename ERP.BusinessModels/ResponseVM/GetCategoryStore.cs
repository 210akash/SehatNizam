namespace ERP.BusinessModels.ResponseVM
{
    public class GetCategoryStore
    {
        public long CategoryId { get; set; }

        public long StoreId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
