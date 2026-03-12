using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class SaleMaterial : BaseEntityHistory
    {
        public string Code { get; set; }
        public DateTime? Date { get; set; }
  
        public long DealershipId { get; set; }
        public virtual Dealership Dealership { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }


        public string Remarks { get; set; }
        public virtual List<SaleMaterialDetail> SaleMaterialDetail { get; set; }
    }
}
