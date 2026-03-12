using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class ApproveIGPHandler : IRequestHandler<ApproveIGPQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveIGPHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveIGPQuery request, CancellationToken cancellationToken)
        {
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IGP.StatusId = 3;
            IGP.ModifiedDate = DateTime.Now;
            IGP.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IGP>().Update(IGP);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
