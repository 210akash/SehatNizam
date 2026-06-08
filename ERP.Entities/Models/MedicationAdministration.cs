using System;
namespace ERP.Entities.Models
{
    public class MedicationAdministration : BaseEntity
    {
        public DateTime AdministrationTime { get; set; }
        public long AdmissionRoundMedicationId { get; set; }
        public Guid AdministeredById { get; set; }
        public AspNetUsers AdministeredBy { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public AdmissionRoundMedication AdmissionRoundMedication { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
