namespace ERP.Entities.Models
{
    public class Currency : BaseEntity
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
