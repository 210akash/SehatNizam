namespace ERP.Entities.Models
{
    public class OrderProcess : BaseEntity
    {
        public long? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public long? CancelDispatchId { get; set; }
        public virtual CancelDispatch CancelDispatch { get; set; }

        public long? FromStatusId { get; set; } // Shop placing the order
        public virtual Status FromStatus { get; set; }

        public long? ToStatusId { get; set; } // Shop placing the order
        public virtual Status ToStatus { get; set; }

        public string Comments { get; set; }
        public string Reference { get; set; }
        public string Department { get; set; }

        public string TransactionId { get; set; }
        public bool? IsReject { get; set; }

        public virtual AspNetUsers CreatedBy { get; set; }
        public virtual AspNetUsers ModifiedBy { get; set; }
    }
}
