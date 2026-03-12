using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dispatch.Command
{
    public class SaveDispatchCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public long StatusId { get; set; }
        public long ProjectId { get; set; }
        public long VehicleId { get; set; }
        public string Remarks { get; set; }
        public int? BiltyNo { get; set; }
        public decimal? FreightCharges { get; set; }
        public List<SaveDispatchOrderCommand> DispatchOrder { get; set; }
    }

    public class SaveDispatchDetailCommand
    {
        public long Id { get; set; }
        public long OrderItemId { get; set; }
        public long Quantity { get; set; }
        public long? DispatchOrderId { get; set; }
        public long? CostSheetId { get; set; }
    }

    public class SaveDispatchOrderCommand
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string DCCode { get; set; }
        public decimal OrderFreightCharges { get; set; }
        public long? DispatchId { get; set; }
        public List<SaveDispatchDetailCommand> DispatchDetail { get; set; }
    }
}
