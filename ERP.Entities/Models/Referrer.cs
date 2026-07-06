namespace ERP.Entities.Models
{
    public class Referrer : BaseEntity
    {
        public string Name { get; set; }
        public string Hospital { get; set; }
        public string PhoneNo { get; set; }
        public long? CompanyId { get; set; }
        public long? AccountId { get; set; }
        public virtual Account Account { get; set; }
        public long? AccountGroupId { get; set; }
        public virtual AccountGroup AccountGroup { get; set; }
        public bool IsGroup { get; set; }
        public virtual Company Company { get; set; }
    }
}
