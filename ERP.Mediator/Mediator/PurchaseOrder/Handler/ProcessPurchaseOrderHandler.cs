using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class ProcessPurchaseOrderHandler : IRequestHandler<ProcessPurchaseOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessPurchaseOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessPurchaseOrderQuery request, CancellationToken cancellationToken)
        {
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PurchaseOrder.StatusId = 2;
            PurchaseOrder.ModifiedDate = DateTime.Now;
            PurchaseOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseOrder>().Update(PurchaseOrder);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
