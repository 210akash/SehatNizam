using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Query
{
    public class DeleteAdmissionPackageMasterQuery : IRequest<bool>
    {
        public DeleteAdmissionPackageMasterQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
