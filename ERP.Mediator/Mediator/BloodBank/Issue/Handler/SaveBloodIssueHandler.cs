using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.Issue.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Handler
{
    public class SaveBloodIssueHandler : IRequestHandler<SaveBloodIssueCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveBloodIssueHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveBloodIssueCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == request.Id);

            if (existing == null)
            {
                return await CreateIssueAsync(request);
            }

            return await UpdateIssueAsync(request, existing);
        }

        private async Task<long> CreateIssueAsync(SaveBloodIssueCommand request)
        {
            if (request.BloodRequestId <= 0 || request.BloodUnitId <= 0 || request.BloodCrossMatchId <= 0)
            {
                return 400;
            }

            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodRequestId && x.IsActive == true && x.IsDelete == false);

            if (bloodRequest == null || bloodRequest.Status != (int)BloodRequestStatus.CrossMatched)
            {
                return 409;
            }

            var crossMatch = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodCrossMatchId && x.IsActive == true && x.IsDelete == false);

            if (crossMatch == null
                || crossMatch.Result != (int)BloodCrossMatchResult.Compatible
                || crossMatch.BloodRequestId != request.BloodRequestId
                || crossMatch.BloodUnitId != request.BloodUnitId)
            {
                return 409;
            }

            var duplicateIssue = await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.BloodRequestId == request.BloodRequestId);

            if (duplicateIssue != null)
            {
                return 409;
            }

            var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.BloodUnitId && x.IsActive == true);

            if (unit == null || unit.Status != (int)BloodUnitStatus.Reserved)
            {
                return 409;
            }

            var entity = mapper.Map<Entities.Models.BloodIssue>(request);
            entity.CreatedById = sessionProvider.Session.LoggedInUserId;
            entity.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodIssue>().Add(entity);

            unit.Status = (int)BloodUnitStatus.Issued;
            unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unit.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);

            bloodRequest.Status = (int)BloodRequestStatus.Issued;
            bloodRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
            bloodRequest.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodRequest>().Update(bloodRequest);

            unitOfWork.SaveChanges();
            return 200;
        }

        private async Task<long> UpdateIssueAsync(SaveBloodIssueCommand request, Entities.Models.BloodIssue existing)
        {
            if (existing.BloodRequestId != request.BloodRequestId
                || existing.BloodUnitId != request.BloodUnitId
                || existing.BloodCrossMatchId != request.BloodCrossMatchId)
            {
                return 409;
            }

            var entity = mapper.Map<Entities.Models.BloodIssue>(request);
            entity.CreatedById = existing.CreatedById;
            entity.CreatedDate = existing.CreatedDate;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            entity.ModifiedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.BloodIssue>().Update(entity);
            unitOfWork.SaveChanges();
            return 200;
        }
    }
}
