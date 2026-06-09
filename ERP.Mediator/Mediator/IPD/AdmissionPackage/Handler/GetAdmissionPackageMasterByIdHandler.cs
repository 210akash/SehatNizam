using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Handler
{
    public class GetAdmissionPackageMasterByIdHandler : IRequestHandler<GetAdmissionPackageMasterByIdQuery, GetAdmissionPackageMaster>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAdmissionPackageMasterByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetAdmissionPackageMaster> Handle(GetAdmissionPackageMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var package = await unitOfWork.Repository<Entities.Models.AdmissionPackageMaster>()
                .GetFirstAsNoTrackingAsync(
                    y => y.Id == request.Id && y.IsActive == true,
                    null,
                    null,
                    "AdmissionPackageDetail,AdmissionPackageDetail.Service");

            if (package?.AdmissionPackageDetail != null)
            {
                package.AdmissionPackageDetail = package.AdmissionPackageDetail
                    .Where(d => d.IsActive && !d.IsDelete)
                    .ToList();
            }

            return mapper.Map<GetAdmissionPackageMaster>(package);
        }
    }
}
