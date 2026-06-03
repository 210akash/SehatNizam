namespace ERP.Entities.Models
{
    public class Referrer : BaseEntity
    {
        public string Name { get; set; }
        public string Hospital { get; set; }
        public string PhoneNo { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
