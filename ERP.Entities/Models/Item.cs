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
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Model { get; set; }
        public int Make { get; set; }
        public decimal ExcessQtyPer { get; set; }
        public decimal OpeningQty { get; set; }

        public string Image { get; set; }

        public long UOMId { get; set; }
        public virtual UOM UOM { get; set; }

        public long ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
