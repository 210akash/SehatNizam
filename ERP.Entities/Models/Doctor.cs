using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Entities.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum AppointmentType { Online, Walkin }
    public enum ConsultationStatus { Pending, Completed }

    public class User
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }        // nullable, not unique
        public DateTime? EmailVerifiedAt { get; set; }
        public string? MrnNumber { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? Cnic { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternateContact { get; set; }
        public string? City { get; set; }
        public string? FullAddress { get; set; }
        public int? Role { get; set; }
        public string? RememberToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // navigation
        public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> ConfirmedAppointments { get; set; } = new List<Appointment>();
        public ICollection<Triage> TriagesTaken { get; set; } = new List<Triage>();
    }

    public class Appointment
    {
        public long Id { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public AppointmentType AppointmentType { get; set; } = AppointmentType.Walkin;
        public string? PriorityLevel { get; set; } = "normal";
        public string Department { get; set; } = "";
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long? ConfirmedBy { get; set; }
        public string VisitType { get; set; } = "0";
        public string? Reason { get; set; }
        public decimal VisitFee { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal TotalPayable { get; set; } = 0m;
        public string PaymentMethod { get; set; } = "cash";
        public string PaymentStatus { get; set; } = "pending";
        public string? QrCode { get; set; }
        public string? ConfirmationNotes { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public int Status { get; set; } = 0;
        public string? TokenNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // navigation
        public User Patient { get; set; } = null!;
        public User Doctor { get; set; } = null!;
        public User? Confirmer { get; set; }
        public ICollection<Triage> Triages { get; set; } = new List<Triage>();
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<PatientProblem> Problems { get; set; } = new List<PatientProblem>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<AppointmentAttachment> Attachments { get; set; } = new List<AppointmentAttachment>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public ICollection<Radiology> RadiologyOrders { get; set; } = new List<Radiology>();
    }

    public class Triage
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long PatientId { get; set; }
        public decimal? Temperature { get; set; }
        public int? Pulse { get; set; }
        public decimal? SystolicBp { get; set; }   // decimal(5,2)
        public int? DiastolicBp { get; set; }
        public int? Spo2 { get; set; }
        public decimal? Weight { get; set; }
        public decimal? HeightFeet { get; set; }   // decimal(5,2)
        public int? HeightInches { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? Bmi { get; set; }
        public int? BloodSugar { get; set; }
        public string? SugarType { get; set; }
        public string? Priority { get; set; }
        public string? PriorityDescription { get; set; }
        public string? ChiefComplaint { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int TriageScore { get; set; } = 0;
        public string TriageCategory { get; set; } = "stable";
        public DateTime? TakenAt { get; set; }
        public long? TakenBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
        public User Patient { get; set; } = null!;
        public User? Nurse { get; set; }
    }

    public class Consultation
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string? Subjective { get; set; }
        public string? Objective { get; set; }
        public string? Assessment { get; set; }
        public string? Plan { get; set; }
        public DateOnly? FollowUpDate { get; set; }
        public ConsultationStatus Status { get; set; } = ConsultationStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }

    public class PatientProblem
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string Problem { get; set; } = "";
        public int Status { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }

    public class Prescription
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string DrugName { get; set; } = "";
        public string? Dosage { get; set; }
        public string? DrugCode { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }

    public class AppointmentAttachment
    {
        public long Id { get; set; }
        public long? AppointmentId { get; set; }
        public long? PatientId { get; set; }
        public string? Attachment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment? Appointment { get; set; }
        public User? Patient { get; set; }
    }

    public class LabOrder
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string? TestName { get; set; }
        public string? TestCode { get; set; }
        public string Status { get; set; } = "ordered";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }

    public class Radiology
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string TestName { get; set; } = "";
        public string? TestCode { get; set; }
        public string? ClinicalNotes { get; set; }
        public int Status { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }

    // Spatie-like permission tables (simplified)
    public class Permission
    {
        public long Id { get; set; }
        [MaxLength(125)] public string Name { get; set; } = "";
        [MaxLength(125)] public string GuardName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<ModelPermission> ModelPermissions { get; set; } = new List<ModelPermission>();
    }

    public class Role
    {
        public long Id { get; set; }
        public long? TeamId { get; set; }   // only if teams enabled
        [MaxLength(125)] public string Name { get; set; } = "";
        [MaxLength(125)] public string GuardName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<ModelRole> ModelRoles { get; set; } = new List<ModelRole>();
    }

    public class RolePermission
    {
        public long PermissionId { get; set; }
        public long RoleId { get; set; }

        public Permission Permission { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }

    public class ModelPermission
    {
        public long PermissionId { get; set; }
        public string ModelType { get; set; } = "";
        public long ModelId { get; set; }
        public long? TeamId { get; set; }

        public Permission Permission { get; set; } = null!;
    }

    public class ModelRole
    {
        public long RoleId { get; set; }
        public string ModelType { get; set; } = "";
        public long ModelId { get; set; }
        public long? TeamId { get; set; }

        public Role Role { get; set; } = null!;
    }

}
