using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetItem
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ItemTypeId { get; set; }
        public GetItemType ItemType { get; set; }

        public long RecordLevel { get; set; }

        public decimal Rate { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Model { get; set; }
        public int Make { get; set; }
        public decimal ExcessQtyPer { get; set; }
        public decimal OpeningQty { get; set; }
        public long UOMId { get; set; }
        public virtual GetUOM UOM { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
