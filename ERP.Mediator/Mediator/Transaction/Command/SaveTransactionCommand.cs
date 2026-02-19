using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.Transaction.Command
{
    public class SaveTransactionCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceNumber { get; set; }
        public long VoucherTypeId { get; set; }
        public string Remarks { get; set; }
        public long StatusId { get; set; }

        #region Bank Vouchers

        public string ChequeNo { get; set; }
        public string ChequeTitle { get; set; }
        public DateTime? ChequeDate { get; set; }
        public DateTime? ChequeClearDate { get; set; }
        public string PaidReceiveBy { get; set; }

        #endregion
        public virtual List<SaveTransactionDetailCommand> TransactionDetails { get; set; }
        public virtual List<SaveTransactionDocumentCommand> TransactionDocuments { get; set; }
    }

    public class SaveTransactionDetailCommand
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long AccountId { get; set; }
        public long DepartmentId { get; set; }
        public long ProjectId { get; set; }
        public decimal Quantity { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }

    public class SaveTransactionDocumentCommand
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
