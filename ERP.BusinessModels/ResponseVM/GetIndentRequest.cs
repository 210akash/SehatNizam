using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetIndentRequest
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public GetUser CreatedBy { get; set; }

        public GetUser ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        public GetUser ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        public long DepartmentId { get; set; }
        public GetDepartment Department { get; set; }

        public long ProjectId { get; set; }
        public GetProject Project { get; set; }

        public long StatusId { get; set; }
        public GetStatus Status { get; set; }

        public long StoreId { get; set; }
        public GetStore Store { get; set; }

        public long IndentTypeId { get; set; }
        public GetIndentType IndentType { get; set; }

        public virtual List<GetIndentRequestDetail> IndentRequestDetail { get; set; }
    }

    public class GetIndentRequestDetail
    {
        public long Id { get; set; }
        public long IndentRequestId { get; set; }
        public GetIndentRequest IndentRequest { get; set; }
        
        public long ItemId { get; set; }
        public GetItem Item { get; set; }
        public decimal Required { get; set; }
        public string Description { get; set; }
    }
}
