using System;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetCostSheet
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string BatchNo { get; set; }
        public DateTime Date { get; set; }
        public long ItemId { get; set; }
        public GetItem Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal TollFillRate { get; set; }
        public decimal AdvSaleTaxPer { get; set; }
        public decimal AdvFEDPer { get; set; }
        public decimal TMaterialCost { get; set; }
        public decimal TFillingPerPet { get; set; }
        public decimal CostPerPet { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public virtual List<GetCostSheetDetail> CostSheetDetail { get; set; }
    }

    public class GetCostSheetDetail
    {
        public long Id { get; set; }
        public long CostSheetId { get; set; }
        public GetCostSheet CostSheet { get; set; }
        
        public long ItemId { get; set; }
        public GetItem Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
