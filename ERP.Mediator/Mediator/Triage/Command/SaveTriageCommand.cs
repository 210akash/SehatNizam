using MediatR;
using System;

namespace ERP.Mediator.Mediator.Triage.Command
{
    public class SaveTriageCommand : IRequest<long>
    {
        public long Id { get; set; }   // Triage Id (for update)
        public long AppointmentId { get; set; }
        public Guid? NurseId { get; set; }
        public decimal? Temperature { get; set; }
        public int? Pulse { get; set; }
        public decimal? SystolicBp { get; set; }   // decimal(5,2)
        public int? DiastolicBp { get; set; }
        public int? Spo2 { get; set; }
        public decimal? Weight { get; set; }
        public decimal? HeightFeet { get; set; }   // decimal(5,2)
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
    }
}