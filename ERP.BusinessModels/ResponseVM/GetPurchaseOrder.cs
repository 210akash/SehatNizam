using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPurchaseOrder
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long PurchaseDemandId { get; set; }
        public GetPurchaseDemand PurchaseDemand { get; set; }

        public long VendorId { get; set; }
        public virtual GetVendor Vendor { get; set; }

        public long CurrencyId { get; set; }
        public virtual GetCurrency Currency { get; set; }

        public long ShipmentModeId { get; set; }
        public virtual GetShipmentMode ShipmentMode { get; set; }

        public long PaymentModeId { get; set; }
        public virtual GetPaymentMode PaymentMode { get; set; }

        public DateTime DeliveryDate { get; set; }

        public long DeliveryTermsId { get; set; }
        public GetDeliveryTerms DeliveryTerms { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal OtherCharges { get; set; }
        public string RefNo { get; set; }

        public bool IsFixDiscount { get; set; }
        public long Discount { get; set; }

        public string StatusRemarks { get; set; }

        public string Remarks { get; set; }
        public bool Iscspo { get; set; }
        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public virtual List<GetPurchaseOrderDetail> PurchaseOrderDetail { get; set; }
    }

    public class GetPurchaseOrderDetail
    {
        public long Id { get; set; }
        public long PurchaseOrderId { get; set; }
        public long PurchaseDemandDetailId { get; set; }
        public virtual GetPurchaseDemandDetail PurchaseDemandDetail { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Value { get; set; }
        public decimal FED { get; set; }
        public decimal GST { get; set; }
        public string Description { get; set; }
    }
}
