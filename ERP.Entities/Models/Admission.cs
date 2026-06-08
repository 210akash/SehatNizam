using System;
namespace ERP.Entities.Models
{
    public class Admission : BaseEntity
    {
        public long AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public long? WardId { get; set; }
        public Ward Ward { get; set; }

        public long? BedId { get; set; }
        public Bed Bed { get; set; }
        
        public AppointmentStatus Status { get; set; }
        public long StatusId { get; set; }
    }
}
