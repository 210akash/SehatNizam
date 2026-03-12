namespace ERP.Entities.Models
{
    public class IssuanceDetail : BaseEntity
    {
        public long IssuanceId { get; set; }
        public virtual Issuance Issuance { get; set; }

        public long IndentRequestDetailId { get; set; }
        public virtual IndentRequestDetail IndentRequestDetail { get; set; }
        public long? CostSheetId { get; set; }
        public virtual CostSheet CostSheet { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
