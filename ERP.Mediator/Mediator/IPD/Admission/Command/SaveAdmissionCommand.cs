using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.IPD.Admission.Command
{
    public class SaveAdmissionCommand : IRequest<long>
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

        // Existing patient
        public long BedId { get; set; }
        public decimal TotalPackageAmount { get; set; }
        public string AdmissionDiagnosis { get; set; }
        public List<SaveAppointmentPaymentCommand> AppointmentPayments { get; set; }
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
}