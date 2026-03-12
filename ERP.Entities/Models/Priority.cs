namespace ERP.Entities.Models
{
    public class Priority : BaseEntity
    {
        public string Name { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
