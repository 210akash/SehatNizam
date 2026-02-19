using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class ApprovePurchaseDemandHandler : IRequestHandler<ApprovePurchaseDemandQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApprovePurchaseDemandHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApprovePurchaseDemandQuery request, CancellationToken cancellationToken)
        {
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PurchaseDemand.StatusId = 3;
            PurchaseDemand.ModifiedDate = DateTime.Now;
            PurchaseDemand.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseDemand>().Update(PurchaseDemand);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
