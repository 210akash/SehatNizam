using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class ApproveInspectionHandler : IRequestHandler<ApproveInspectionQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveInspectionHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveInspectionQuery request, CancellationToken cancellationToken)
        {
            var Inspection = await unitOfWork.Repository<Entities.Models.Inspection>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Inspection.StatusId = 3;
            Inspection.ModifiedDate = DateTime.Now;
            Inspection.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Inspection>().Update(Inspection);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
