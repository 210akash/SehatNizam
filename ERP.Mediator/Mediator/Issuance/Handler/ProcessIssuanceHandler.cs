using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class ProcessIssuanceHandler : IRequestHandler<ProcessIssuanceQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessIssuanceHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessIssuanceQuery request, CancellationToken cancellationToken)
        {
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Issuance.StatusId = 2;
            Issuance.ModifiedDate = DateTime.Now;
            Issuance.ModifiedById = sessionProvider.Session.LoggedInUserId;
            Issuance.ProcessedDate = DateTime.Now;
            Issuance.ProcessedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Issuance>().Update(Issuance);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
