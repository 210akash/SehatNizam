using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class SalaryHead : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsTaxable { get; set; } = false;

        [Required]
        public SalaryHeadType Type { get; set; } // Earning or Deduction
    }

    public enum SalaryHeadType
    {
        Earning = 1,
        Deduction = 2
    }
}
