using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetShopType
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long ShopId { get; set; }
        public GetShop Shop { get; set; }
    }
}
