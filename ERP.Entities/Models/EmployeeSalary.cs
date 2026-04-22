using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class EmployeeSalary : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public virtual AspNetUsers Employee { get; set; }

        public long SalaryHeadId { get; set; }
        public virtual SalaryHead SalaryHead { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime EffectiveFrom { get; set; } = DateTime.Now;
    }
}
