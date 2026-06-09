using System;
using System.Collections.Generic;
namespace ERP.Entities.Models
{
    public class Admission : BaseEntity
    {
        public long AppointmentId { get; set; }
        public long AdmissionPackageMasterId { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string AdmissionDiagnosis { get; set; }
      
        public decimal TotalPackageAmount { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }

        public AdmissionPackageMaster AdmissionPackageMaster { get; set; }
        public AppointmentStatus Status { get; set; }
        public long StatusId { get; set; }
        public ICollection<AdmissionRound> AdmissionRounds { get; set; }
        public ICollection<AdmissionBed> AdmissionBeds { get; set; }
    }
}
