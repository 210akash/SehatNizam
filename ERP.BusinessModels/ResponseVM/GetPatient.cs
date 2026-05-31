using System;
using System.Collections.Generic;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPatient
    {
        public long Id { get; set; }
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
        public GetCity City { get; set; }
        public long ProjectId { get; set; }
        public GetProject Project { get; set; }
        public List<Appointment> PatientAppointments { get; set; }
    }
}
