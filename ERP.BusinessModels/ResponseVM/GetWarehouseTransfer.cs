using System;
using System.Collections.Generic;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetWarehouseTransfer
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }

        public long TransferToId { get; set; }
        public virtual GetProject TransferTo { get; set; }

        public long TransferFromId { get; set; }
        public virtual GetProject TransferFrom { get; set; }

        public long CompanyId { get; set; }
        public virtual GetCompany Company { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public virtual List<GetWarehouseTransferDetail> WarehouseTransferDetail { get; set; }
    }

    public class GetWarehouseTransferDetail
    {
        public long Id { get; set; }

        public long WarehouseTransferId { get; set; }
        public virtual GetWarehouseTransfer WarehouseTransfer { get; set; }

        public long ItemId { get; set; }
        public virtual GetItem Item { get; set; }

        public long? CostSheetId { get; set; }
        public virtual GetCostSheet CostSheet { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
