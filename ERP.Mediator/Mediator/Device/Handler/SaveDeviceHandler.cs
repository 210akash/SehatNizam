using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Device.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Device.Handler
{
    public class SaveDeviceHandler : IRequestHandler<SaveDeviceCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveDeviceHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception dex)
            {

                throw;
            }
        }

        public async Task<long> Handle(SaveDeviceCommand request, CancellationToken cancellationToken)
        {
            var Device = await unitOfWork.Repository<Entities.Models.Device>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Device>()
                .GetAsync(x => x.Name.ToLower() == request.Name.ToLower()
                               && x.IsActive == true
                               && x.IsDelete == false
                               && x.Id != request.Id
                               && x.CompanyId == sessionProvider.Session.CompanyId);

            if (checkDuplicate.Count() == 0)
            {
                if (Device == null)
                {
                    var _Device = mapper.Map<Entities.Models.Device>(request);
                    _Device.CompanyId = sessionProvider.Session.CompanyId;
                    _Device.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Device.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Device>().Add(_Device);
                    SaveChanges();
                }
                else
                {
                    var _Device = mapper.Map<Entities.Models.Device>(request);
                    _Device.CompanyId = Device.CompanyId;
                    _Device.CreatedById = Device.CreatedById;
                    _Device.CreatedDate = Device.CreatedDate;
                    _Device.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Device.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Device>().Update(_Device);
                    SaveChanges();
                }

                return 200; // Success code for adding/updating
            }
            else
            {
                return 409; // Conflict code for duplicate
            }
        }
    }
}