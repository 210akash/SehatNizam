using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Query
{
    public class DeleteAdmissionServicesQuery : IRequest<bool>
    {
        public DeleteAdmissionServicesQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}