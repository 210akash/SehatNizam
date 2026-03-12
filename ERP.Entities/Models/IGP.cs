using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class IGP : BaseEntity
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }
        // public bool Manual { get; set; } = false;

        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNo { get; set; }
        public string DriverCnic { get; set; }
        public string BiltyNo { get; set; }

        public long? IGPTypeId { get; set; }
        public virtual IGPType IGPType { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public virtual List<IGPDetails> IGPDetails { get; set; }
    }
}
