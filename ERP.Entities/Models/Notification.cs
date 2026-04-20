using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.Models
{
    public class Notification : BaseEntity
    {
        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        public bool IsExpired => DateTime.Now > ExpireDate;
    }
}
