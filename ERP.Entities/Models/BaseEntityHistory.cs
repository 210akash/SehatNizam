using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class BaseEntityHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Guid? CreatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public Guid? ProcessedById { get; set; }
        public virtual AspNetUsers ProcessedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }

        public Guid? ApprovedById { get; set; }
        public virtual AspNetUsers ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual AspNetUsers CreatedBy { get; set; }
        public virtual AspNetUsers ModifiedBy { get; set; }
    }
}
