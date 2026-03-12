namespace ERP.Entities.Models
{
    public class RetailOrderProcess : BaseEntity
    {
        public long? RetailOrderId { get; set; }
        public virtual RetailOrder RetailOrder { get; set; }

        public long? FromStatusId { get; set; } // Shop placing the order
        public virtual Status FromStatus { get; set; }

        public long? ToStatusId { get; set; } // Shop placing the order
        public virtual Status ToStatus { get; set; }

        public string Comments { get; set; }

        public virtual AspNetUsers CreatedBy { get; set; }
        public virtual AspNetUsers ModifiedBy { get; set; }
    }
}
