using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Patient : BaseEntity
    {
        public string MRN { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string SecondaryPhoneNo { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Age { get; set; }
        public long? CityId { get; set; }
        public City City { get; set; }
        public long ProjectId { get; set; }
        public Project Project { get; set; }

        #region Appoinment 
        public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        #endregion
    }
}
