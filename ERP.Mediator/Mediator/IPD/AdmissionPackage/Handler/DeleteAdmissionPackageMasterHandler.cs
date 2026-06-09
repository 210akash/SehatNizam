using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Handler
{
    public class DeleteAdmissionPackageMasterHandler : IRequestHandler<DeleteAdmissionPackageMasterQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteAdmissionPackageMasterHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAdmissionPackageMasterQuery request, CancellationToken cancellationToken)
        {
            var package = await unitOfWork.Repository<AdmissionPackageMaster>()
                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            if (package == null)
            {
                return false;
            }

            package.IsDelete = true;
            package.IsActive = false;
            package.DeleteDate = DateTime.Now;
            package.ModifiedDate = DateTime.Now;
            package.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<AdmissionPackageMaster>().Update(package);

            var details = await unitOfWork.Repository<AdmissionPackageDetail>()
                .GetPagingWhereAsNoTrackingAsync(
                    y => y.AdmissionPackageMasterId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

            foreach (var detail in details)
            {
                detail.IsDelete = true;
                detail.IsActive = false;
                detail.DeleteDate = DateTime.Now;
                detail.ModifiedDate = DateTime.Now;
                detail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<AdmissionPackageDetail>().Update(detail);
            }

            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
