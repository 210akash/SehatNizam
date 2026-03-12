namespace ERP.Entities.Models
{
    public class Section : BaseEntity
    {
        public string Name { get; set; }

        public long RowId { get; set; }
        public virtual Row Row { get; set; }
    }
}
