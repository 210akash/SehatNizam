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
            // 1️  Grab the PurchaseOrder *with* its details and keep it tracked
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetFirstAsync(y => y.Id == request.Id, null, null, "PurchaseOrderDetail");

            if (PurchaseOrder is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            PurchaseOrder.IsDelete = true;
            PurchaseOrder.IsActive = false;
            PurchaseOrder.DeleteDate = now;
            PurchaseOrder.ModifiedDate = now;
            PurchaseOrder.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in PurchaseOrder.PurchaseOrderDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.PurchaseOrder>().Update(PurchaseOrder);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
