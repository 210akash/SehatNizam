using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class PayrollDetail : BaseEntity
    {
        public long PayrollId { get; set; }
        public virtual Payroll Payroll { get; set; }

        public long EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }

        public long SalaryHeadId { get; set; }
        public virtual SalaryHead SalaryHead { get; set; }

        [Required]
        public decimal Amount { get; set; }

        // Snapshot fields for reporting
        public string SalaryHeadName { get; set; }
        public SalaryHeadType SalaryHeadType { get; set; }
    }
}
