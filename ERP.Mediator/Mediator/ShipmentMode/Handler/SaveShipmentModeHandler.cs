using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShipmentMode.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Handler
{
    public class SaveShipmentModeHandler : IRequestHandler<SaveShipmentModeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveShipmentModeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveShipmentModeCommand, long>.Handle(SaveShipmentModeCommand request, CancellationToken cancellationToken)
        {
            var ShipmentMode = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.ShipmentMode>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (ShipmentMode == null)
                {
                    var _ShipmentMode = mapper.Map<Entities.Models.ShipmentMode>(request);
                    _ShipmentMode.CompanyId = sessionProvider.Session.CompanyId;
                    _ShipmentMode.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _ShipmentMode.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ShipmentMode>().Add(_ShipmentMode);
                    SaveChanges();
                }
                else
                {
                    var _ShipmentMode = mapper.Map<Entities.Models.ShipmentMode>(request);
                    _ShipmentMode.CompanyId = ShipmentMode.CompanyId;
                    _ShipmentMode.CreatedById = ShipmentMode.CreatedById;
                    _ShipmentMode.CreatedDate = ShipmentMode.CreatedDate;
                    _ShipmentMode.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _ShipmentMode.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ShipmentMode>().Update(_ShipmentMode);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}