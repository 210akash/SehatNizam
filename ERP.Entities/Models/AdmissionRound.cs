using System;
namespace ERP.Entities.Models
{
    public class AdmissionRound : BaseEntity
    {
        public long AdmissionId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime RoundDateTime { get; set; }
        public string ChiefComplaint { get; set; }
        public string Notes { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public string Instructions { get; set; }
        public Admission Admission { get; set; }
       // public ICollection<AdmissionRoundMedication> Medications { get; set; }
    }
}
