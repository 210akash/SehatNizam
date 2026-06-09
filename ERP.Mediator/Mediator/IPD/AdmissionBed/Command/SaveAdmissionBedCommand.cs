using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Command
{
    public class SaveAdmissionBedCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long AdmissionId { get; set; }
        public long BedId { get; set; }
    }
}
