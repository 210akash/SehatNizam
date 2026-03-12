using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDealershipAccountLedger
    {
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
        public string ReferenceNumber { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public string Remarks { get; set; }
    }
}
