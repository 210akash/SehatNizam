using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Query
{
    public class GetAdmissionPackageMasterByIdQuery : IRequest<GetAdmissionPackageMaster>
    {
        public GetAdmissionPackageMasterByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
