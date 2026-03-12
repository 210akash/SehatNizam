using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IGP.Command
{
    public class SaveIGPCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long PurchaseOrderId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNo { get; set; }
        public string DriverCnic { get; set; }
        public string BiltyNo { get; set; }
        public long? IGPTypeId { get; set; }
        public List<SaveIGPDetailsCommand> IGPDetails { get; set; }
    }

    public class SaveIGPDetailsCommand
    {
        public long Id { get; set; }
        public long IGPId { get; set; }
        public decimal Received { get; set; }
        public long PurchaseOrderDetailId { get; set; }
    }
}
