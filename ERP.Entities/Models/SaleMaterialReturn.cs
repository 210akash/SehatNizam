using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class SaleMaterialReturn : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long SaleMaterialId { get; set; }
        public virtual SaleMaterial SaleMaterial { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        public virtual List<SaleMaterialReturnDetail> SaleMaterialReturnDetail { get; set; }
    }
}
