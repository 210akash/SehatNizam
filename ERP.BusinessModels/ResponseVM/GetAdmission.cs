using System;
using System.Collections.Generic;
namespace ERP.BusinessModels.ResponseVM
{
    public class GetAdmission
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public GetAppointment Appointment { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string AdmissionDiagnosis { get; set; }
        public long? BedId { get; set; }
        public GetBed Bed { get; set; }
        public decimal TotalPackageAmount { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public GetAppointmentStatus Status { get; set; }
        public long StatusId { get; set; }
        public ICollection<GetAdmissionBed> AdmissionBeds { get; set; }
        //public ICollection<AdmissionRound> AdmissionRounds { get; set; }
    }
}
