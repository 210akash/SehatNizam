namespace ERP.Entities.Models
{
    public class PatientProblem : BaseEntity
    {
        public long AppointmentId { get; set; }
        public string Problem { get; set; }
        public string Onset { get; set; }
        public Status Status { get; set; }
        public long StatusId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
