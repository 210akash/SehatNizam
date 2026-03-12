using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRetailOrderProcess
    {
        public long RetailOrderId { get; set; }
        public GetRetailOrder RetailOrder { get; set; }

        public long? FromStatusId { get; set; }
        public GetStatus FromStatus { get; set; }

        public long? ToStatusId { get; set; }
        public GetStatus ToStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Comments { get; set; }

        public Guid? CreatedById { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
    }
}
