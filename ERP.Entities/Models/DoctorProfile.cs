using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.Entities.Models
{
    public class DoctorProfile : BaseEntity
    {
        public Guid DoctorId { get; set; }

        public string PMDCNumber { get; set; }

        public string Qualification { get; set; }

        public int ExperienceYears { get; set; }

        public string Biography { get; set; }

        public string Specialization { get; set; }

        public decimal? ConsultationFee { get; set; }
        public decimal? HospitalPercentage { get; set; }      // e.g. 50%

        public bool IsAvailableForOPD { get; set; } = true;

        public bool IsAvailableForIPD { get; set; } = true;

        public long? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public long? AccountGroupId { get; set; }
        public virtual AccountGroup AccountGroup { get; set; }
        public bool IsGroup { get; set; }

        // Dynamic fields - stored as JSON string in database
        public string CustomFieldsJson { get; set; }

        [NotMapped]
        public JsonDocument CustomFields
        {
            get => string.IsNullOrEmpty(CustomFieldsJson) ? null : JsonDocument.Parse(CustomFieldsJson);
            set => CustomFieldsJson = value?.ToString();
        }

        public AspNetUsers Doctor { get; set; }

        public ICollection<DoctorServiceFee> DoctorServiceFees { get; set; }  = new List<DoctorServiceFee>();
    }
}
