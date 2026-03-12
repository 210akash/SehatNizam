using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Region.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class DeleteRegionHandler : IRequestHandler<DeleteRegionQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRegionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRegionQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Zone>().GetExistsAsync(y => y.RegionId == request.Id && y.IsActive))
            {
                var region = await unitOfWork.Repository<Entities.Models.Region>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                region.IsDelete = true;
                region.IsActive = false;
                region.ModifiedDate = DateTime.Now;
                region.DeleteDate = DateTime.Now;
                region.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Region>().Update(region);
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
