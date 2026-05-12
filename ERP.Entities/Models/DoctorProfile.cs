using System;
using System.Collections.Generic;
using System.Text.Json;

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

        public bool IsAvailableForOPD { get; set; } = true;

        public bool IsAvailableForIPD { get; set; } = true;

        // Dynamic fields
        public JsonDocument CustomFields { get; set; }

        public AspNetUsers Doctor { get; set; }

        public ICollection<DoctorServiceFee> DoctorServiceFees { get; set; }  = new List<DoctorServiceFee>();
    }
}
