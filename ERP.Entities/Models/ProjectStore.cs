using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class ProjectStore : BaseEntity
    {
        public long ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StoreId { get; set; }
        public virtual Store Store { get; set; }

        public virtual List<AspNetUsers> Users { get; set; }
    }
}
