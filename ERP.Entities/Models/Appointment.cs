using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Appointment : BaseEntity
    {
        public string? TokenNumber { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public Department Department { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long? ConfirmedById { get; set; }
        public VisitType VisitType { get; set; }
        public long? VisitTypeId { get; set; }
        public string? Reason { get; set; }
        public decimal VisitFee { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal TotalPayable { get; set; } = 0m;
        public PaymentMode PaymentMode { get; set; }
        public long PaymentModeId { get; set; }

        public Status PaymentStatus { get; set; }
        public long PaymentStatusId { get; set; }

        public string? QrCode { get; set; }
        public string? ConfirmationNotes { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public Status Status { get; set; }
        public long StatusId { get; set; }

        // navigation
        public AspNetUsers Patient { get; set; } = null!;
        public AspNetUsers Doctor { get; set; } = null!;
        public AspNetUsers? ConfirmedBy { get; set; }
        public ICollection<Triage> Triages { get; set; } = new List<Triage>();
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<PatientProblem> Problems { get; set; } = new List<PatientProblem>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<AppointmentAttachment> Attachments { get; set; } = new List<AppointmentAttachment>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public ICollection<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
    }
}
