using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Dispatch : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public long VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string Remarks { get; set; }

        public int? BiltyNo { get; set; }
        public decimal? FreightCharges { get; set; }

        public virtual List<DispatchOrder> DispatchOrder { get; set; }
    }
}
