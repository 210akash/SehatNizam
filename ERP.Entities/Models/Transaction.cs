using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Transaction : BaseEntityHistory
    {
        [MaxLength(10)]
        public string Code { get; set; }
        public DateTime Date { get; set; }

        public string ReferenceNumber { get; set; }

        public long VoucherTypeId { get; set; }
        public virtual VoucherType VoucherType { get; set; }

        public string Remarks { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        #region Bank Vouchers

        public string ChequeNo { get; set; }
        public string ChequeTitle { get; set; }
        public DateTime? ChequeDate { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public string PaidReceiveBy { get; set; }

        #endregion

        public virtual List<TransactionDetail> TransactionDetails { get; set; }
        public virtual List<TransactionDocument> TransactionDocuments { get; set; }
    }
}
