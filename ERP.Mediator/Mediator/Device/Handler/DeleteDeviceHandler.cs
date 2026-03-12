using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Device.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Device.Handler
{
    public class DeleteDeviceHandler : IRequestHandler<DeleteDeviceQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteDeviceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteDeviceQuery request, CancellationToken cancellationToken)
        {
            var Device = await unitOfWork.Repository<Entities.Models.Device>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Device.IsDelete = true;
            Device.IsActive = false;
            Device.DeleteDate = DateTime.Now;
            Device.ModifiedDate = DateTime.Now;
            Device.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Device>().Update(Device);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
