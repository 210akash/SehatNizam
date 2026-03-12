using System;
using System.Collections.Generic;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSaleMaterial
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }

        public long DealershipId { get; set; }
        public virtual GetDealership Dealership { get; set; }

        public long CompanyId { get; set; }
        public virtual GetCompany Company { get; set; }

        public long ProjectId { get; set; }
        public virtual GetProject Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public virtual List<GetSaleMaterialDetail> SaleMaterialDetail { get; set; }
    }

    public class GetSaleMaterialDetail
    {
        public long Id { get; set; }

        public long SaleMaterialId { get; set; }
        public virtual GetSaleMaterial SaleMaterial { get; set; }

        public long ItemId { get; set; }
        public virtual GetItem Item { get; set; }

        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
