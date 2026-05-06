using System;
namespace ERP.Entities.Models
{
    public class DoctorServiceFee : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public long ServiceId { get; set; }

        // Fee Options
        public decimal? FixedAmount { get; set; }     // e.g. 2000
        public decimal? Percentage { get; set; }      // e.g. 60%

        // Optional overrides
        public decimal? HospitalShare { get; set; }   // optional
        public decimal? DoctorShare { get; set; }     // optional

        // Navigation
        public AspNetUsers Doctor { get; set; }
        public Service Service { get; set; }
    }
}
