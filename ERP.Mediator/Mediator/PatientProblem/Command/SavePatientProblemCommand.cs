using MediatR;

namespace ERP.Mediator.Mediator.PatientProblem.Command
{
    public class SavePatientProblemCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string Problem { get; set; }
        public long StatusId { get; set; }
    }
}