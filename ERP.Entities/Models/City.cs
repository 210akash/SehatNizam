using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}
