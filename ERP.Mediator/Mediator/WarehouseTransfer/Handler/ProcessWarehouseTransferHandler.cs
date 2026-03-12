using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.WarehouseTransfer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Handler
{
    public class ProcessWarehouseTransferHandler : IRequestHandler<ProcessWarehouseTransferQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessWarehouseTransferHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessWarehouseTransferQuery request, CancellationToken cancellationToken)
        {
            var WarehouseTransfer = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            WarehouseTransfer.StatusId = 2;
            WarehouseTransfer.ModifiedDate = DateTime.Now;
            WarehouseTransfer.ModifiedById = sessionProvider.Session.LoggedInUserId;

            WarehouseTransfer.ProcessedDate = DateTime.Now;
            WarehouseTransfer.ProcessedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.WarehouseTransfer>().Update(WarehouseTransfer);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
