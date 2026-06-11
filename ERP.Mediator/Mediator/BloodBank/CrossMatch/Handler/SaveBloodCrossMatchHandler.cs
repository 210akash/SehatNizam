using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.CrossMatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Handler
{
    public class SaveBloodCrossMatchHandler : IRequestHandler<SaveBloodCrossMatchCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodCrossMatchHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodCrossMatchCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == request.Id);

            if (existing == null)
            {
                return await AssignUnitAsync(request);
            }

            return await UpdateResultAsync(request, existing);
        }

        private async Task<long> AssignUnitAsync(SaveBloodCrossMatchCommand request)
        {
            if (request.BloodRequestId <= 0 || request.BloodUnitId <= 0)
            {
                return 400;
            }

            if (request.Result != 0 && request.Result != (int)BloodCrossMatchResult.InProcess)
            {
                return 400;
            }

            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodRequestId && x.IsActive == true && x.IsDelete == false);

            if (bloodRequest == null || bloodRequest.Status != (int)BloodRequestStatus.Pending)
            {
                return 409;
            }

            var inProcessExists = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false
                    && x.BloodRequestId == request.BloodRequestId
                    && x.Result == (int)BloodCrossMatchResult.InProcess);

            if (inProcessExists != null)
            {
                return 409;
            }

            var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodUnitId && x.IsActive == true && x.IsDelete == false);

            if (unit == null || unit.Status != (int)BloodUnitStatus.Available)
            {
                return 409;
            }

            var unitHeld = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false
                    && x.BloodUnitId == request.BloodUnitId
                    && x.Result == (int)BloodCrossMatchResult.InProcess);

            if (unitHeld != null)
            {
                return 409;
            }

            var entity = mapper.Map<Entities.Models.BloodCrossMatch>(request);
            entity.Result = (int)BloodCrossMatchResult.InProcess;
            entity.CreatedById = sessionProvider.Session.LoggedInUserId;
            entity.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodCrossMatch>().Add(entity);

            unit.Status = (int)BloodUnitStatus.Reserved;
            unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unit.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<long> UpdateResultAsync(SaveBloodCrossMatchCommand request, Entities.Models.BloodCrossMatch existing)
        {
            if (existing.Result != (int)BloodCrossMatchResult.InProcess)
            {
                return 409;
            }

            if (request.Result != (int)BloodCrossMatchResult.Compatible
                && request.Result != (int)BloodCrossMatchResult.Incompatible)
            {
                return 400;
            }

            var entity = mapper.Map<Entities.Models.BloodCrossMatch>(request);
            entity.BloodRequestId = existing.BloodRequestId;
            entity.BloodUnitId = existing.BloodUnitId;
            entity.CreatedById = existing.CreatedById;
            entity.CreatedDate = existing.CreatedDate;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodCrossMatch>().Update(entity);

            var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetFirstAsNoTrackingAsync(x => x.Id == existing.BloodUnitId && x.IsActive == true);

            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(x => x.Id == existing.BloodRequestId && x.IsActive == true);

            if (request.Result == (int)BloodCrossMatchResult.Compatible)
            {
                if (unit != null)
                {
                    unit.Status = (int)BloodUnitStatus.Reserved;
                    unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unit.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);
                }

                if (bloodRequest != null)
                {
                    bloodRequest.Status = (int)BloodRequestStatus.CrossMatched;
                    bloodRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    bloodRequest.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.BloodRequest>().Update(bloodRequest);
                }
            }
            else
            {
                if (unit != null)
                {
                    unit.Status = (int)BloodUnitStatus.Available;
                    unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unit.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);
                }
            }

            unitOfWork.SaveChanges();
            return 200;
        }
    }
}
