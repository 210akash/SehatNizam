using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ERP.Entities.Models
{
    public class Room : BaseEntity
    {
        [MaxLength(4)]
        public string Code { get; set; }
        public string Name { get; set; }
        public long WardId { get; set; }
        public string Description { get; set; }
        public Ward Ward { get; set; }
        public ICollection<Bed> Beds { get; set; }
    }
}
