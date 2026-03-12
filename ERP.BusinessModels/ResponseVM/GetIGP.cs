using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetIGP
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long PurchaseOrderId { get; set; }
        public virtual GetPurchaseOrder PurchaseOrder { get; set; }

        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNo { get; set; }
        public string DriverCnic { get; set; }
        public string BiltyNo { get; set; }

        public long? IGPTypeId { get; set; }
        public virtual GetIGPType IGPType { get; set; }

        public long? ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetIGPDetails> IGPDetails { get; set; }
    }

    public class GetIGPDetails
    {
        public long Id { get; set; }

        public long IGPId { get; set; }
        public virtual GetIGP IGP { get; set; }

        public decimal Received { get; set; }

        public long PurchaseOrderDetailId { get; set; }
        public virtual GetPurchaseOrderDetail PurchaseOrderDetail { get; set; }
    }
}
