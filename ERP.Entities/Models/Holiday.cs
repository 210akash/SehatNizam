using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Holiday : BaseEntity
    {
        [MaxLength(100)]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
