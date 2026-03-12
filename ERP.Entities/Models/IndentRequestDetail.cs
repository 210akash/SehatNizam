namespace ERP.Entities.Models
{
    public class IndentRequestDetail : BaseEntity
    {
        public long IndentRequestId { get; set; }
        public virtual IndentRequest IndentRequest { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public decimal Required { get; set; }

        public string Description { get; set; }
    }
}
