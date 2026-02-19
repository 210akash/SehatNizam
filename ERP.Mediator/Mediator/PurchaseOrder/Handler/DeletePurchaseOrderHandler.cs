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
    public class DeletePurchaseOrderHandler : IRequestHandler<DeletePurchaseOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePurchaseOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePurchaseOrderQuery request, CancellationToken cancellationToken)
        {
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PurchaseOrder.IsDelete = true;
            PurchaseOrder.IsActive = false;
            PurchaseOrder.DeleteDate = DateTime.Now;
            PurchaseOrder.ModifiedDate = DateTime.Now;
            PurchaseOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseOrder>().Update(PurchaseOrder);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
