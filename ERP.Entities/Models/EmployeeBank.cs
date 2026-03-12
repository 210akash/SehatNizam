using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class EmployeeBank : BaseEntity
    {
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}
