using System.ComponentModel.DataAnnotations;
namespace ERP.Entities.Models
{
    public class Bed : BaseEntity
    {
        [MaxLength(2)]
        public string Code { get; set; }
        public long RoomId { get; set; }
        public string BedNo { get; set; }
        public decimal DailyCharges { get; set; }
        public bool IsOccupied { get; set; }
        public Room Room { get; set; }
    }
}
