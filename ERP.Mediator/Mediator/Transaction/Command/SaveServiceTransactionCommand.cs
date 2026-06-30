using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Transaction.Command
{
    public class SaveServiceTransactionCommand : IRequest<long>
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

        #region Sale Vouchers

        public long? OrderId { get; set; }

        #endregion

        #region Purchase Vouchers

        public long? GRNDetailId { get; set; }

        #endregion

        #region Service/Appoinments Payments Vouchers
        public long AppoinmentsPayments { get; set; }

        #endregion

        public virtual List<SaveTransactionDetailCommand> TransactionDetails { get; set; }
        public virtual List<SaveTransactionDocumentCommand> TransactionDocuments { get; set; }
    }
}
