using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class CostSheet : BaseEntityHistory
    {
        public string Code { get; set; }
  
        public long ItemId { get; set; }
        public virtual Item Item { get; set; }
        public decimal Quantity { get; set; }
        public decimal TollFillRate { get; set; }
        public decimal AdvSaleTaxPer { get; set; }
        public decimal AdvFEDPer { get; set; }
        public decimal TMaterialCost { get; set; }
        public decimal TFillingPerPet { get; set; }
        public decimal CostPerPet { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }
        public virtual List<CostSheetDetail> CostSheetDetail { get; set; }
        public virtual List<GRNDetail> GRNDetails { get; set; }
        public virtual List<DispatchDetail> DispatchDetail { get; set; }
        public virtual List<WarehouseTransferDetail> WarehouseTransferDetail { get; set; }
    }
}
