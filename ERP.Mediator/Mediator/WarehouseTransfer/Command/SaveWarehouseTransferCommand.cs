using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Command
{
    public class SaveWarehouseTransferCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }
        public long TransferToId { get; set; }
        public long? TransferFromId { get; set; }
        public long StatusId { get; set; }
        public virtual List<SaveWarehouseTransferDetailCommand> WarehouseTransferDetail { get; set; }
    }

    public class SaveWarehouseTransferDetailCommand
    {
        public long Id { get; set; }
        public long WarehouseTransferId { get; set; }
        public long ItemId { get; set; }
        public long? CostSheetId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
