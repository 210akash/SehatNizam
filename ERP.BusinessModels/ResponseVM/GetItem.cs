using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetItem
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public long RecordLevel { get; set; }
        public decimal Rate { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int Model { get; set; }
        public int Make { get; set; }
        public decimal ExcessQtyPer { get; set; }
        public decimal OpeningQty { get; set; }
        public int RetailPrice { get; set; }
        public int TradePrice { get; set; }
        public int DistributorPrice { get; set; }
        public int DistributorPromo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public decimal QuantityInPack { get; set; }

        public int LeftQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int TransitQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal StockQty { get; set; }
        public string Store { get; set; }

        public long ItemTypeId { get; set; }
        public GetItemType ItemType { get; set; }

        public long UOMId { get; set; }
        public virtual GetUOM UOM { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public bool IsGroupItem { get; set; }
        public GetUser CreatedBy { get; set; }
        public List<GetCategoryStore> CategoryStore { get; set; }
        public List<GetItemGroup> ItemGroup { get; set; }
    }
}
