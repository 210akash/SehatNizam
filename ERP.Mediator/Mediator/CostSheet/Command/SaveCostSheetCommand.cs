using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.CostSheet.Command
{
    public class SaveCostSheetCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal TollFillRate { get; set; }
        public decimal AdvSaleTaxPer { get; set; }
        public decimal AdvFEDPer { get; set; }
        public decimal TMaterialCost { get; set; }
        public decimal TFillingPerPet { get; set; }
        public decimal CostPerPet { get; set; }
        public long StatusId { get; set; }
        public virtual List<SaveCostSheetDetailCommand> CostSheetDetail { get; set; }
    }

    public class SaveCostSheetDetailCommand
    {
        public long Id { get; set; }
        public long CostSheetId { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
