using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.PurchaseOrder.Command
{
    public class SavePurchaseOrderCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long VendorId { get; set; }
        public long CurrencyId { get; set; }
        public long ShipmentModeId { get; set; }
        public long PaymentModeId { get; set; }
        public DateTime DeliveryDate { get; set; }

        public long DeliveryTermsId { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal OtherCharges { get; set; }
        public string RefNo { get; set; }
        public bool IsFixDiscount { get; set; }
        public long Discount { get; set; }
        public string Remarks { get; set; }

        public long CompanyId { get; set; }

        public long StatusId { get; set; }
        public virtual List<SavePurchaseOrderDetailCommand> PurchaseOrderDetail { get; set; }
    }

    public class SavePurchaseOrderDetailCommand
    {
        public long Id { get; set; }
        public long PurchaseOrderId { get; set; }
        public long PurchaseDemandDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Value { get; set; }
        public decimal FED { get; set; }
        public decimal GST { get; set; }
        public string Description { get; set; }
    }
}
