using ERP.Mediator.Mediator.Appointment.Command;
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
        public long AdmissionPackageMasterId { get; set; }
        public long PaymentModeId { get; set; }
        public string AdmissionDiagnosis { get; set; }
        public List<SaveAppointmentPaymentCommand> AppointmentPayments { get; set; }
    }
}