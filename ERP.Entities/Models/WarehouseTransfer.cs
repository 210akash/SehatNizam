using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class WarehouseTransfer : BaseEntityHistory
    {
        public string Code { get; set; }
        public DateTime? Date { get; set; }
  
        public long TransferToId { get; set; }
        public virtual Project TransferTo { get; set; }

        public long TransferFromId { get; set; }
        public virtual Project TransferFrom { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string Remarks { get; set; }
        public virtual List<WarehouseTransferDetail> WarehouseTransferDetail { get; set; }
    }
}
