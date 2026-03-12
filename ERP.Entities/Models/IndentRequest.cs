using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class IndentRequest : BaseEntityHistory
    {
        [MaxLength(7)]
        public string Code { get; set; }
        public DateTime RequiredDate { get; set; }

        public long? StoreId { get; set; }
        public virtual Store Store { get; set; }

        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public long? IndentTypeId { get; set; }
        public virtual IndentType IndentType { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long StatusId { get; set; }
        public virtual Status Status { get; set; }

        public string StatusRemarks { get; set; }

        public string Remarks { get; set; }

        public virtual List<IndentRequestDetail> IndentRequestDetail { get; set; }
    }
}
