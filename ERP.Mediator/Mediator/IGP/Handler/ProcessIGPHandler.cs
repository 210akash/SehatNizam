using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class ProcessIGPHandler : IRequestHandler<ProcessIGPQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessIGPHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessIGPQuery request, CancellationToken cancellationToken)
        {
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IGP.StatusId = 2;
            IGP.ModifiedDate = DateTime.Now;
            IGP.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IGP>().Update(IGP);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
