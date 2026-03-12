using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Rack : BaseEntity
    {
        public string Name { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
