namespace ERP.Entities.Models
{
    public class BloodRack : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public long BloodFridgeId { get; set; }
        public virtual BloodFridge BloodFridge { get; set; }
    }
}
