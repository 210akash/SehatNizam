using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Vehicle.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vehicle.Handler
{
    public class SaveVehicleHandler : IRequestHandler<SaveVehicleCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveVehicleHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveVehicleCommand, long>.Handle(SaveVehicleCommand vehicle, CancellationToken cancellationToken)
        {
            var shopType = await unitOfWork.Repository<Entities.Models.Vehicle>().GetFirstAsNoTrackingAsync(x => x.Id == vehicle.Id);
            if (vehicle.IsHeadOfficeVehicle == true)
            {
                vehicle.DealershipId = null;
            }
            else
            {
                vehicle.LogisticPartner = null;
            }
            if (shopType == null)
            {
                var _shopType = mapper.Map<Entities.Models.Vehicle>(vehicle);
                _shopType.CreatedById = sessionProvider.Session.LoggedInUserId;
                _shopType.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Vehicle>().Add(_shopType);
                SaveChanges();
            }
            else
            {
                var _shopType = mapper.Map<Entities.Models.Vehicle>(vehicle);
                _shopType.CreatedById = shopType.CreatedById;
                _shopType.CreatedDate = shopType.CreatedDate;
                _shopType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _shopType.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Vehicle>().Update(_shopType);
                SaveChanges();
            }
            return 200;

        }
    }
}