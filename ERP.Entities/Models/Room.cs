using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ERP.Entities.Models
{
    public class Room : BaseEntity
    {
        [MaxLength(2)]
        public string Code { get; set; }
        public long WardId { get; set; }
        public string Description { get; set; }
        public Ward Ward { get; set; }
        public ICollection<Bed> Beds { get; set; }
    }
}
