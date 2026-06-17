using System;
namespace ERP.Entities.Models
{
    public class DischargeCertificate : BaseEntity
    {
        public long AdmissionId { get; set; }
        public Admission Admission { get; set; }
        public DateTime? OperationDeliveryDateTime { get; set; }
        public string Diagnosis { get; set; }
        public string Hopi { get; set; }
        public string ExaminationAndFindings { get; set; }
        public string InvestigationsResults { get; set; }
        public string Procedure { get; set; }
        public string SurgeonName { get; set; }
        public string OperativeFindings { get; set; }
        public string OperationNotes { get; set; }
        public string ConditionAtDischarge { get; set; }
        public string TreatmentAdvisedAtDischarge { get; set; }
        public DateTime? ProposedFollowUpDateTime { get; set; }
        public string DietAndInstructions { get; set; }
        public Guid? DischargeDoctorId { get; set; }
        public AspNetUsers DischargeDoctor { get; set; }
        public DateTime DischargeDateTime { get; set; }
    }
}
