using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetTriage
    {
        public long Id { get; set; }
        public GetAppointment Appointment { get; set; }
        public long AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? NurseId { get; set; }
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
        public int TriageScore { get; set; }
        public long TriageCategoryId { get; set; }
        public DateTime? TakenAt { get; set; }
        public GetPatient Patient { get; set; }
        public GetUser Nurse { get; set; }
        public GetSugarType SugarType { get; set; }
        public GetTriagePriority TriagePriority { get; set; }
        public GetTriageCategory TriageCategory { get; set; }
    }
}
