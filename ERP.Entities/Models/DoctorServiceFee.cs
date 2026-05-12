using System;
namespace ERP.Entities.Models
{
    public class DoctorServiceFee : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public long ServiceId { get; set; }
        public decimal? DoctorPercentage { get; set; }      // e.g. 50%
        public AspNetUsers Doctor { get; set; }
        public Service Service { get; set; }
    }
}
