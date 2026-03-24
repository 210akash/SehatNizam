namespace ERP.Entities.Models
{
    public class SugarType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Company Company { get; set; }
        public long CompanyId { get; set; }
    }
}
