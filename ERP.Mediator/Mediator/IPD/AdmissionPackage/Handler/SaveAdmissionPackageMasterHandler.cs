using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IPD.AdmissionPackage.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.IPD.AdmissionPackage.Handler
{
    public class SaveAdmissionPackageMasterHandler : IRequestHandler<SaveAdmissionPackageMasterCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAdmissionPackageMasterHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveAdmissionPackageMasterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return 400;
            }

            if (request.AdmissionPackageDetail == null || !request.AdmissionPackageDetail.Any())
            {
                return 400;
            }

            var checkDuplicate = await unitOfWork.Repository<AdmissionPackageMaster>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id);

            if (checkDuplicate.Any())
            {
                return 409;
            }

            var package = await unitOfWork.Repository<AdmissionPackageMaster>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.Id);

            if (package == null)
            {
                var newPackage = mapper.Map<AdmissionPackageMaster>(request);
                newPackage.CreatedById = sessionProvider.Session.LoggedInUserId;
                newPackage.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                newPackage.CreatedDate = DateTime.Now;

                foreach (var detail in newPackage.AdmissionPackageDetail)
                {
                    detail.CreatedById = sessionProvider.Session.LoggedInUserId;
                    detail.CreatedDate = DateTime.Now;
                }

                unitOfWork.Repository<AdmissionPackageMaster>().Add(newPackage);
                SaveChanges();
            }
            else
            {
                var masterUpdate = request;
                var detailUpdate = masterUpdate.AdmissionPackageDetail ?? new List<SaveAdmissionPackageDetailCommand>();
                masterUpdate.AdmissionPackageDetail = null;

                var updatedPackage = mapper.Map<AdmissionPackageMaster>(masterUpdate);
                updatedPackage.CreatedById = package.CreatedById;
                updatedPackage.CreatedDate = package.CreatedDate;
                updatedPackage.ProjectId = package.ProjectId;
                updatedPackage.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updatedPackage.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<AdmissionPackageMaster>().Update(updatedPackage);
                SaveChanges();

                var existingDetails = await unitOfWork.Repository<AdmissionPackageDetail>()
                    .GetPagingWhereAsNoTrackingAsync(
                        y => y.AdmissionPackageMasterId == request.Id && y.IsActive == true,
                        null, null, null, null, null).Item1.ToListAsync();

                var previousDetailIds = existingDetails.Select(y => y.Id).ToList();
                var currentDetailIds = detailUpdate.Select(y => y.Id).ToList();
                var deletedDetailIds = previousDetailIds.Except(currentDetailIds).ToList();

                foreach (var deletedDetailId in deletedDetailIds)
                {
                    var detail = existingDetails.FirstOrDefault(y => y.Id == deletedDetailId);
                    if (detail != null)
                    {
                        detail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        detail.DeleteDate = DateTime.Now;
                        detail.IsActive = false;
                        detail.IsDelete = true;
                        unitOfWork.Repository<AdmissionPackageDetail>().Update(detail);
                    }
                }

                foreach (var detailCommand in detailUpdate)
                {
                    if (detailCommand.Id != 0)
                    {
                        var existingDetail = await unitOfWork.Repository<AdmissionPackageDetail>()
                            .GetFirstAsync(x => x.Id == detailCommand.Id);

                        existingDetail.ServiceId = detailCommand.ServiceId;
                        existingDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        existingDetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<AdmissionPackageDetail>().Update(existingDetail);
                    }
                    else
                    {
                        var newDetail = mapper.Map<AdmissionPackageDetail>(detailCommand);
                        newDetail.AdmissionPackageMasterId = request.Id;
                        newDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        newDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<AdmissionPackageDetail>().Add(newDetail);
                    }
                }

                SaveChanges();
            }

            return 200;
        }
    }
}
