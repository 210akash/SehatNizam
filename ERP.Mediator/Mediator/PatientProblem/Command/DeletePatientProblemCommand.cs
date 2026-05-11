using MediatR;

namespace ERP.Mediator.Mediator.PatientProblem.Command
{
    public class DeletePatientProblemCommand : IRequest<bool>
    {
        public DeletePatientProblemCommand(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}