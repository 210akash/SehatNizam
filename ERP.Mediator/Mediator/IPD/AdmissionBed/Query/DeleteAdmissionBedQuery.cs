using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Query
{
    public class DeleteAdmissionBedQuery : IRequest<bool>
    {
        public DeleteAdmissionBedQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}