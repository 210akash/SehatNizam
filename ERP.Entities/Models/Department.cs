using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ERP.Entities.Models
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<AspNetUsers> Users { get; set; }
    }
}
