namespace ERP.Entities.Models
{
    public class ItemGroup : BaseEntity
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Item Item { get; set; }
    }
}
