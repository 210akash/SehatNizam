using System;
using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class Patient : BaseEntity
    {
        public long ProjectId { get; set; }
        public Project Project { get; set; }
        public string MRN { get; set; }
        public long PatientMasterId { get; set; }
        public PatientMaster PatientMaster { get; set; }
        #region Appoinment 
        public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        #endregion
    }
}
