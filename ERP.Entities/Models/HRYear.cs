using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class HRYear : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
