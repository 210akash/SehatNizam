using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class GRN : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }

        public long InspectionId { get; set; }
        public virtual Inspection Inspection { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string Remarks { get; set; }

        #region Purchase Invoice

        [MaxLength(7)]
        public string InvoiceNo { get; set; }
        public decimal? WHTPercentage { get; set; }
        public string Comments { get; set; }

        public Guid? InvoiceProcessedById { get; set; }
        public virtual AspNetUsers InvoiceProcessedBy { get; set; }
        public DateTime? InvoiceProcessedDate { get; set; }

        public Guid? InvoiceAuditVerifiedById { get; set; }
        public virtual AspNetUsers InvoiceAuditVerifiedBy { get; set; }
        public DateTime? InvoiceAuditVerifiedDate { get; set; }

        public Guid? InvoiceApprovedById { get; set; }
        public virtual AspNetUsers InvoiceApprovedBy { get; set; }
        public DateTime? InvoiceApprovedDate { get; set; }

        public long? InvoiceStatusId { get; set; }
        public virtual Status InvoiceStatus { get; set; }

        #endregion

        public virtual List<GRNDetail> GRNDetail { get; set; }
    }
}
