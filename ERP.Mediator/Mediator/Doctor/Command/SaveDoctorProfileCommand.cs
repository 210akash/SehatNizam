
using MediatR;
using System;

namespace ERP.Mediator.Mediator.Doctor.Command
{
    public class SaveDoctorProfileCommand : IRequest<int>
    {
        public long Id { get; set; }
        public Guid DoctorId { get; set; }
        public string PMDCNumber { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string Biography { get; set; }
        public string Specialization { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? HospitalPercentage { get; set; }
        public bool IsAvailableForOPD { get; set; }
        public bool IsAvailableForIPD { get; set; }
        public bool IsGroup { get; set; }
        public long? AccountId { get; set; }
        public long? AccountGroupId { get; set; }
        public string CustomFieldsJson { get; set; }
    }
}
