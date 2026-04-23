namespace ERP.Entities.Models
{
    public class SalaryTaxSlab : BaseEntity
    {
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}
