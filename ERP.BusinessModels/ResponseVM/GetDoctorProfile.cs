using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDoctorProfile
    {
        public long Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string PMDCNumber { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string Biography { get; set; }
        public string Specialization { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? HospitalPercentage { get; set; }
        public bool IsAvailableForOPD { get; set; }
        public bool IsAvailableForIPD { get; set; }
        public virtual GetAccount Account { get; set; }
        public long? AccountId { get; set; }
        public virtual GetAccountGroup AccountGroup { get; set; }
        public long? AccountGroupId { get; set; }
        public string CustomFieldsJson { get; set; }
        public bool IsActive { get; set; }
    }
}
