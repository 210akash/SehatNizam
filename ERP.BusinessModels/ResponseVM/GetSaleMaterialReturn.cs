using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSaleMaterialReturn
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long SaleMaterialId { get; set; }
        public virtual GetSaleMaterial SaleMaterial { get; set; }

        public long? ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public string Remarks { get; set; }

        public virtual List<GetSaleMaterialReturnDetail> SaleMaterialReturnDetail { get; set; }
    }

    public class GetSaleMaterialReturnDetail
    {
        public long Id { get; set; }
        public long SaleMaterialReturnId { get; set; }
        public decimal Quantity { get; set; }
        public long SaleMaterialDetailId { get; set; }
        public virtual GetSaleMaterialDetail SaleMaterialDetail { get; set; }
    }
}
