using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDispatch
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ProcessedDate { get; set; }
        public GetUser ProcessedBy { get; set; }

        public DateTime ApprovedDate { get; set; }
        public GetUser ApprovedBy { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long VehicleId { get; set; }
        public GetVehicle Vehicle { get; set; }

        public long? ProjectId { get; set; }
        public GetProject Project { get; set; }

        public string Remarks { get; set; }

        public int? BiltyNo { get; set; }
        public decimal? FreightCharges { get; set; }

        public List<GetDispatchOrder> DispatchOrder { get; set; }
    }

    public class GetDispatchOrder
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
        public GetOrder Order { get; set; }

        public string DCCode { get; set; }
        public decimal OrderFreightCharges { get; set; }

        public decimal DistributorAmount { get; set; }
        public decimal DistributorMargin { get; set; }
        public decimal TradeMargin { get; set; }
        public decimal TradePromo { get; set; }

        public long DispatchId { get; set; }
        public GetDispatch Dispatch { get; set; }

        public long? StatusId { get; set; }
        public GetStatus Status { get; set; }

        public Guid? ReceivedById { get; set; }
        public GetUser ReceivedBy { get; set; }

        public DateTime? ReceivedDate { get; set; }
        public Guid? PrintById { get; set; }
        public DateTime? PrintDate { get; set; }

        public List<GetDispatchDetail> DispatchDetail { get; set; }
    }

    public class GetDispatchDetail
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }

        public long OrderItemId { get; set; }
        public GetOrderItems OrderItem { get; set; }

        public long Quantity { get; set; }

        public long DispatchOrderId { get; set; }
        public GetDispatchOrder DispatchOrder { get; set; }

        public long? CostSheetId { get; set; }
        public GetCostSheet CostSheet { get; set; }
    }

}
