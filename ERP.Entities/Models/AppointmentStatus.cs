using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class AppointmentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? CreatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public virtual AspNetUsers CreatedBy { get; set; }
        public virtual AspNetUsers ModifiedBy { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
