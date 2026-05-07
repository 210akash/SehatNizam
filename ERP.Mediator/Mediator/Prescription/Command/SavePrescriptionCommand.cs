using MediatR;

namespace ERP.Mediator.Mediator.Prescription.Command
{
    public class SavePrescriptionCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
    }
}
