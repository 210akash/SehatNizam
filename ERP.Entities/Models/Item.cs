using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Item : BaseEntity
    {
        [MaxLength(11)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long RecordLevel { get; set; }
        public int LeadTime { get; set; }
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
        public decimal QuantityInPack { get; set; }

        public string Image { get; set; }

        public long UOMId { get; set; }
        public virtual UOM UOM { get; set; }

        public long ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public bool IsGroupItem { get; set; }
        public virtual ICollection<PriceGroupDetails> PriceGroupDetails { get; set; }
        public virtual ICollection<ItemGroup> ItemGroup { get; set; }

    }
}
