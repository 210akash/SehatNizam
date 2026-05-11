using MediatR;

namespace ERP.Mediator.Mediator.Prescription.Command
{
    public class DeletePrescriptionCommand : IRequest<bool>
    {
        public DeletePrescriptionCommand(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}