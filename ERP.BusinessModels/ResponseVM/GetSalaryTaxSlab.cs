namespace ERP.BusinessModels.ResponseVM
{
    public class GetSalaryTaxSlab
    {
        public long Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}
