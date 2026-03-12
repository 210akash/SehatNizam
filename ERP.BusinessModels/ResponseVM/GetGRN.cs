using ERP.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetGRN
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public long InspectionId { get; set; }
        public virtual GetInspection Inspection { get; set; }

        public long StatusId { get; set; }
        public virtual GetStatus Status { get; set; }

        public long InvoiceStatusId { get; set; }
        public virtual GetStatus InvoiceStatus { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
        public GetUser ModifiedBy { get; set; }

        #region Purchase Invoice

        [MaxLength(7)]
        public string InvoiceNo { get; set; }
        public decimal? WHTPercentage { get; set; }
        public string Comments { get; set; }

        public Guid? InvoiceProcessedById { get; set; }
        public GetCreatedBy InvoiceProcessedBy { get; set; }
        public DateTime? InvoiceProcessedDate { get; set; }

        public Guid? InvoiceAuditVerifiedById { get; set; }
        public GetCreatedBy InvoiceAuditVerifiedBy { get; set; }
        public DateTime? InvoiceAuditVerifiedDate { get; set; }

        public DateTime? InvoiceApprovedDate { get; set; }
        public Guid? InvoiceApprovedById { get; set; }
        public GetCreatedBy InvoiceApprovedBy { get; set; }

        #endregion

        public string Remarks { get; set; }

        public virtual List<GetGRNDetail> GRNDetail { get; set; }
    }

    public class GetGRNDetail
    {
        public long Id { get; set; }

        public long GRNId { get; set; }
        public virtual GetGRN GRN { get; set; }

        //public long? SectionId { get; set; }
        //public virtual Section Section { get; set; }

        public long? CostSheetId { get; set; }
        public virtual GetCostSheet CostSheet { get; set; }

        public decimal Received { get; set; }

        public string Refernace { get; set; }
        public long? SectionId { get; set; }

        public DateTime ExpireDate { get; set; }

        public long InspectionDetailId { get; set; }
        public virtual GetInspectionDetail InspectionDetail { get; set; }
        public virtual GetSection Section { get; set; }
    }
}
