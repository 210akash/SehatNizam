using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class PurchaseOrder : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public long CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public long ShipmentModeId { get; set; }
        public virtual ShipmentMode ShipmentMode { get; set; }

        public long PaymentModeId { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }

        public DateTime DeliveryDate { get; set; }

        public long DeliveryTermsId { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal OtherCharges { get; set; }
        public string RefNo { get; set; }

        public bool IsFixDiscount { get; set; }
        public bool Iscspo { get; set; }
        public long Discount { get; set; }

        public string StatusRemarks { get; set; }

        public string Remarks { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<PurchaseOrderDetail> PurchaseOrderDetail { get; set; }
        public virtual List<IGP> IGP { get; set; }
    }
}
