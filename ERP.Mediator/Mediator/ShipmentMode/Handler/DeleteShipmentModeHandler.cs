using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShipmentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Handler
{
    public class DeleteShipmentModeHandler : IRequestHandler<DeleteShipmentModeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteShipmentModeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteShipmentModeQuery request, CancellationToken cancellationToken)
        {
            var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            ShipmentMode.IsDelete = true;
            ShipmentMode.IsActive = false;
            ShipmentMode.DeleteDate = DateTime.Now;
            ShipmentMode.ModifiedDate = DateTime.Now;
            ShipmentMode.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ShipmentMode>().Update(ShipmentMode);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
