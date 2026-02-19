using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetGST
    {
        public long Id { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public decimal GSTPer { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
    }
}
