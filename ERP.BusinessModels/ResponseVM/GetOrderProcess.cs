using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetOrderProcess
    {
        public long OrderId { get; set; }
        public GetOrder Order { get; set; }
        public long? FromStatusId { get; set; } // Shop placing the order
        public GetStatus FromStatus { get; set; }
        public long? ToStatusId { get; set; } // Shop placing the order
        public GetStatus ToStatus { get; set; }
        public string Comments { get; set; }
        public string Reference { get; set; }
        public string Department { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public string TransactionId { get; set; }
        public bool? IsReject { get; set; }
    }

    public class GetDealershipOrderProcess
    {
        public string FromStatus { get; set; } // Shop placing the order
        public string ToStatus { get; set; }
        public string Comments { get; set; }
        public string Reference { get; set; }
        public string Department { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsReject { get; set; }
    }
}
