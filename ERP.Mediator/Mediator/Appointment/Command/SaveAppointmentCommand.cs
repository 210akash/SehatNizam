using ERP.Mediator.Mediator.LabOrder.Command;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Appointment.Command
{
    public class SaveAppointmentCommand : IRequest<long>
    {
        public long Id { get; set; }

        // -----------------------------------
        // ENCOUNTER / APPOINTMENT
        // -----------------------------------

        public DateTime AppointmentDate { get; set; }

        public string TokenNumber { get; set; }

        public long ProjectId { get; set; }

        // Department determines flow
        // OPD / LAB / RADIOLOGY / IPD / ER
        public long DepartmentId { get; set; }

        // Self / Family / Insurance etc.
        public long AppointmentTypeId { get; set; }

        // Normal / Urgent
        public long PriorityLevelId { get; set; }

        // First Visit / Follow Up
        public long? VisitTypeId { get; set; }

        // Optional for direct services
        public Guid? DoctorId { get; set; }

        public string Reason { get; set; }

        public string ConfirmationNotes { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public long AppointmentStatusId { get; set; }
        public long? ReferrerId { get; set; }

        // -----------------------------------
        // PATIENT
        // -----------------------------------

        // Existing patient
        public long? PatientId { get; set; }

        // New patient
        public PatientCommand Patient { get; set; }

        // -----------------------------------
        // PAYMENT
        // -----------------------------------

        public List<SaveAppointmentPaymentCommand> AppointmentPayment { get; set; }

        // -----------------------------------
        // LAB
        // -----------------------------------

        public List<SaveLabOrderCommand> LabOrders { get; set; }
            = new();

        // -----------------------------------
        // RADIOLOGY
        // -----------------------------------

        public List<SaveRadiologyOrderCommand> RadiologyOrders { get; set; }
            = new();
    }

    public class SaveAppointmentCommand1 : IRequest<long>
    {
        public long Id { get; set; }   // Appointment Id (for update)

        // Core Fields
        public DateTime AppointmentDate { get; set; }
        public string TokenNumber { get; set; }
        public long AppointmentTypeId { get; set; }
        public long PriorityLevelId { get; set; }
        public long DepartmentId { get; set; }

        public long? PatientId { get; set; }
        public Guid DoctorId { get; set; }

        public long? VisitTypeId { get; set; }

        public string Reason { get; set; }

        public string ConfirmationNotes { get; set; }
        public DateTime? ConfirmedDate { get; set; }

        public long AppointmentStatusId { get; set; }
        public PatientCommand Patient { get; set; }

        // 🔹 Child Collections
        public SaveAppointmentPaymentCommand AppointmentPayment { get; set; }
    }
    public class PatientCommand
    {
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Age { get; set; }
        public long CityId { get; set; }
        public long ProjectId { get; set; }
    }

    public class SaveAppointmentPaymentCommand
    {
        public long Id { get; set; }   // Id (for update)
        public long AppointmentId { get; set; }
        public long ServiceId { get; set; }   // ✔️ ADD THIS ONLY
        public decimal VisitFee { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal TotalPayable { get; set; } = 0m;
        public long PaymentModeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long PaymentStatusId { get; set; }
    }

    public class SaveTriageCommand
    {
        public long Id { get; set; }

        public long AppointmentId { get; set; }

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
}