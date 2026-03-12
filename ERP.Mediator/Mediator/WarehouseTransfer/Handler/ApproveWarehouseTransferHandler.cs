using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.WarehouseTransfer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Handler
{
    public class ApproveWarehouseTransferHandler : IRequestHandler<ApproveWarehouseTransferQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveWarehouseTransferHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, string>> Handle(ApproveWarehouseTransferQuery request, CancellationToken cancellationToken)
        {
            var warehouseTransfer = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            warehouseTransfer.StatusId = 3;
            warehouseTransfer.ApprovedDate = DateTime.Now;
            warehouseTransfer.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.WarehouseTransfer>().Update(warehouseTransfer);
            await unitOfWork.SaveChangesAsync();
            return new Tuple<long, string>(200, "Approved!");
        }
    }
}
