
namespace ERP.Entities.Models
{
    public class Prescription : BaseEntity
    {
        public long AppointmentId { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public string DrugCode { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }

        public Appointment Appointment { get; set; } = null!;
    }
}
