namespace ERP.Entities.Models
{
    public class TransactionDetail : BaseEntity
    {
        public long TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }

        public long AccountId { get; set; }
        public virtual Account Account { get; set; }

        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public long ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public decimal Quantity { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
