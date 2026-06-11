using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.BloodBank.CrossMatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Handler
{
    public class DeleteBloodCrossMatchHandler : IRequestHandler<DeleteBloodCrossMatchQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteBloodCrossMatchHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteBloodCrossMatchQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id && y.IsActive == true && y.IsDelete == false);

            if (entity == null)
            {
                return 404;
            }

            var linkedIssue = await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.BloodCrossMatchId == entity.Id);

            if (linkedIssue != null)
            {
                return 409;
            }

            if (entity.Result == (int)BloodCrossMatchResult.InProcess
                || entity.Result == (int)BloodCrossMatchResult.Compatible)
            {
                var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == entity.BloodUnitId && x.IsActive == true);

                if (unit != null && unit.Status != (int)BloodUnitStatus.Issued)
                {
                    unit.Status = (int)BloodUnitStatus.Available;
                    unit.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unit.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.BloodUnit>().Update(unit);
                }
            }

            if (entity.Result == (int)BloodCrossMatchResult.Compatible)
            {
                var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == entity.BloodRequestId && x.IsActive == true);

                if (bloodRequest != null && bloodRequest.Status == (int)BloodRequestStatus.CrossMatched)
                {
                    bloodRequest.Status = (int)BloodRequestStatus.Pending;
                    bloodRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    bloodRequest.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.BloodRequest>().Update(bloodRequest);
                }
            }

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.DeleteDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.BloodCrossMatch>().Update(entity);
            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
