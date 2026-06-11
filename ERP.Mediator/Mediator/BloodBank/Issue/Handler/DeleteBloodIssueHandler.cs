using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.Issue.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Handler
{
    public class DeleteBloodIssueHandler : IRequestHandler<DeleteBloodIssueQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteBloodIssueHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteBloodIssueQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id && y.IsActive == true && y.IsDelete == false);

            if (entity == null)
            {
                return 404;
            }

            var compatibleCrossMatch = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false
                    && x.BloodRequestId == entity.BloodRequestId
                    && x.BloodUnitId == entity.BloodUnitId
                    && x.Result == (int)BloodCrossMatchResult.Compatible);

            var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                .GetFirstAsNoTrackingAsync(x => x.Id == entity.BloodUnitId && x.IsActive == true);

            if (unit != null && unit.Status == (int)BloodUnitStatus.Issued)
            {
                unit.Status = compatibleCrossMatch != null
                    ? (int)BloodUnitStatus.Reserved
                    : (int)BloodUnitStatus.Available;
                unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unit.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);
            }

            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(x => x.Id == entity.BloodRequestId && x.IsActive == true);

            if (bloodRequest != null && bloodRequest.Status == (int)BloodRequestStatus.Issued)
            {
                bloodRequest.Status = compatibleCrossMatch != null
                    ? (int)BloodRequestStatus.CrossMatched
                    : (int)BloodRequestStatus.Pending;
                bloodRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
                bloodRequest.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.BloodRequest>().Update(bloodRequest);
            }

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.DeleteDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.BloodIssue>().Update(entity);
            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
