using System;

namespace ERP.Entities.Models
{
    public class Consultation : BaseEntity
    {
        public long AppointmentId { get; set; }
        public string Subjective { get; set; }
        public string Objective { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }
        public DateTime FollowUpDate { get; set; }
        public Status Status { get; set; }
        public Appointment Appointment { get; set; }
    }
}
