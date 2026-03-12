namespace ERP.Entities.Models
{
    public class DistributorPriceGroup : BaseEntity
    {
        public long? DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }
        public long? PriceGroupId { get; set; }
        public virtual PriceGroup PriceGroup { get; set; }
    }
}
