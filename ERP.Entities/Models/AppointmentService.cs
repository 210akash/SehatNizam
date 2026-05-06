using System;

namespace ERP.Entities.Models
{
    public class AppointmentService : BaseEntity
    {
        public long AppointmentId { get; set; }
        public long ServiceId { get; set; }

        public Guid DoctorId { get; set; }

        public decimal Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }     // calculated at runtime
        public decimal TotalAmount { get; set; }

        // Revenue split
        public decimal? DoctorAmount { get; set; }
        public decimal? HospitalAmount { get; set; }

        // Navigation
        public Appointment Appointment { get; set; }
        public Service Service { get; set; }
        public AspNetUsers Doctor { get; set; }
    }
}
