using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Appointment.Command
{
    public class SaveAppointmentCommand : IRequest<long>
    {
        public long Id { get; set; }   // Appointment Id (for update)

        // Core Fields
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

        // 🔹 Child Collections
        public List<SaveTriageCommand> Triages { get; set; } = new();
        public List<SaveAppointmentAttachmentCommand> Attachments { get; set; } = new();
    }

    public class SaveTriageCommand
    {
        public long Id { get; set; }

        public long AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? NurseId { get; set; }

        public decimal? Temperature { get; set; }
        public int? Pulse { get; set; }

        public decimal? SystolicBp { get; set; }
        public int? DiastolicBp { get; set; }

        public int? Spo2 { get; set; }

        public decimal? Weight { get; set; }

        public decimal? HeightFeet { get; set; }
        public int? HeightInches { get; set; }
        public decimal? HeightCm { get; set; }

        public decimal? Bmi { get; set; }

        public int? BloodSugar { get; set; }
        public long SugarTypeId { get; set; }

        public long TriagePriorityId { get; set; }

        public string ChiefComplaint { get; set; }
        public string Allergies { get; set; }
        public string Medications { get; set; }
        public string Notes { get; set; }

        public int TriageScore { get; set; } = 0;
        public long TriageCategoryId { get; set; }

        public DateTime? TakenAt { get; set; }
    }

    public class SaveAppointmentAttachmentCommand
    {
        public long Id { get; set; }

        public long AppointmentId { get; set; }
        public long PatientId { get; set; }

        public string Attachment { get; set; }
    }
}