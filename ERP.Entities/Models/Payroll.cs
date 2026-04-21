using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Payroll : BaseEntity
    {
        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        public virtual ICollection<PayrollDetail> PayrollDetails { get; set; }
    }

    public enum PayrollStatus
    {
        Draft = 1,
        Approved = 2,
        Paid = 3
    }
}
