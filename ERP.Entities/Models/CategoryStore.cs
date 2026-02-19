namespace ERP.Entities.Models
{
    public class CategoryStore : BaseEntity
    {
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public long StoreId { get; set; }
        public virtual Store Store { get; set; }
    }
}
