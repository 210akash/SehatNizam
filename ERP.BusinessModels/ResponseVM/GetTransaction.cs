using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetTransaction
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }

        public string ReferenceNumber { get; set; }

        public long VoucherTypeId { get; set; }

        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }

        #region Bank Vouchers

        public string ChequeNo { get; set; }
        public string ChequeTitle { get; set; }
        public DateTime? ChequeDate { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public string PaidReceiveBy { get; set; }

        #endregion

        public virtual List<GetTransactionDetail> TransactionDetails { get; set; }
        public virtual List<GetTransactionDocument> TransactionDocuments { get; set; }
    }

    public class GetTransactionDetail
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public GetTransaction Transaction { get; set; }

        public long AccountId { get; set; }
        public GetAccount Account { get; set; }

        public long DepartmentId { get; set; }
        public GetDepartment Department { get; set; }

        public long ProjectId { get; set; }
        public GetProject Project { get; set; }

        public decimal Quantity { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }

    public class GetTransactionDocument
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }

        public string Path { get; set; }
        [NotMapped]
        public string FileName { get; set; }
    }
}
