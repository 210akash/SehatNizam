using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Zone.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class DeleteZoneHandler : IRequestHandler<DeleteZoneQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteZoneHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteZoneQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Area>().GetExistsAsync(y => y.ZoneId == request.Id && y.IsActive))
            {
                var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                zone.IsDelete = true;
                zone.IsActive = false;
                zone.ModifiedDate = DateTime.Now;
                zone.DeleteDate = DateTime.Now;
                zone.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Zone>().Update(zone);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            }
            else
                return (long)ResponseStatus.Conflict;
        }
    }
}
