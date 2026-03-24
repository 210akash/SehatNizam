using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Appointment : BaseEntity
    {
        public DateTime AppointmentDate { get; set; }
        public string TokenNumber { get; set; }
        public long AppointmentTypeId { get; set; }
        public long PriorityLevelId { get; set; }
        public long DepartmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? ConfirmedById { get; set; }
        public long? VisitTypeId { get; set; }
        public string Reason { get; set; }
        public string QrCode { get; set; }
        public string ConfirmationNotes { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public long AppointmentStatusId { get; set; }

        // navigation
        public AspNetUsers Patient { get; set; } = null!;
        public AspNetUsers Doctor { get; set; } = null!;
        public AspNetUsers? ConfirmedBy { get; set; }
        public Department Department { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public VisitType VisitType { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public ICollection<Triage> Triages { get; set; } = new List<Triage>();
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<PatientProblem> Problems { get; set; } = new List<PatientProblem>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<AppointmentAttachment> Attachments { get; set; } = new List<AppointmentAttachment>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public ICollection<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public ICollection<AppointmentPayment> AppointmentPayments { get; set; } = new List<AppointmentPayment>();
    }
}
