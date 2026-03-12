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
    public class DeleteWarehouseTransferHandler : IRequestHandler<DeleteWarehouseTransferQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteWarehouseTransferHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteWarehouseTransferQuery request, CancellationToken cancellationToken)
        {
            var WarehouseTransfer = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            WarehouseTransfer.IsDelete = true;
            WarehouseTransfer.IsActive = false;
            WarehouseTransfer.DeleteDate = DateTime.Now;
            WarehouseTransfer.ModifiedDate = DateTime.Now;
            WarehouseTransfer.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.WarehouseTransfer>().Update(WarehouseTransfer);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
