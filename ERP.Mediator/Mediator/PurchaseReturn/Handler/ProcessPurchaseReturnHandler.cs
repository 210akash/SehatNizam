using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class ProcessPurchaseReturnHandler : IRequestHandler<ProcessPurchaseReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessPurchaseReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessPurchaseReturnQuery request, CancellationToken cancellationToken)
        {
            var PurchaseReturn = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PurchaseReturn.StatusId = 2;
            PurchaseReturn.ProcessedDate = DateTime.Now;
            PurchaseReturn.ProcessedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseReturn>().Update(PurchaseReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
