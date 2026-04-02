using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAppointment
    {
        public long Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TokenNumber { get; set; }
        public long ProjectId { get; set; }
        public long AppointmentTypeId { get; set; }
        public long PriorityLevelId { get; set; }
        public long DepartmentId { get; set; }
        public long PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? ConfirmedById { get; set; }
        public long? VisitTypeId { get; set; }
        public string Reason { get; set; }
        public string QrCode { get; set; }
        public string ConfirmationNotes { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public long AppointmentStatusId { get; set; }

        // navigation
        public GetProject Project { get; set; }
        public GetPatient Patient { get; set; }
        public GetUser Doctor { get; set; }
        public GetCreatedBy ConfirmedBy { get; set; }
        public GetDepartment Department { get; set; }
        public GetPriorityLevel PriorityLevel { get; set; }
        public GetAppointmentType AppointmentType { get; set; }
        public GetVisitType VisitType { get; set; }
        public GetAppointmentStatus AppointmentStatus { get; set; }
        public List<GetTriage> Triages { get; set; }
        public List<GetConsultation> Consultations { get; set; }
        public List<GetPatientProblem> Problems { get; set; }
        public List<GetPrescription> Prescriptions { get; set; }
        public List<GetAppointmentAttachment> Attachments { get; set; }
        public List<GetLabOrder> LabOrders { get; set; }
        public List<GetRadiologyOrder> RadiologyOrders { get; set; }
        public List<GetAppointmentPayment> AppointmentPayments { get; set; }

    }
}
