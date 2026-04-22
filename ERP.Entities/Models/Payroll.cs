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

        public long StatusId { get; set; }
        public Status Status { get; set; }

        public List<PayrollDetail> PayrollDetails { get; set; }
    }
}
