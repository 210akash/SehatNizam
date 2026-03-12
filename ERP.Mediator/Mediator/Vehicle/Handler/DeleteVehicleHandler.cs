using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Vehicle.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Handler
{
    public class DeleteVehicleHandler : IRequestHandler<DeleteVehicleQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteVehicleHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteVehicleQuery request, CancellationToken cancellationToken)
        {
            //if (!await unitOfWork.Repository<Entities.Models.Shop>().GetExistsAsync(y => y.VehicleId == request.Id && y.IsActive))
            //{
                var vehicle = await unitOfWork.Repository<Entities.Models.Vehicle>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                vehicle.IsDelete = true;
                vehicle.IsActive = false;
                vehicle.ModifiedDate = DateTime.Now;
                vehicle.DeleteDate = DateTime.Now;
                vehicle.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Vehicle>().Update(vehicle);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            //}
            //else
            //    return (long)ResponseStatus.Conflict;
        }
    }
}
