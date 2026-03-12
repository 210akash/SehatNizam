using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class DeleteTerritoryHandler : IRequestHandler<DeleteTerritoryQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteTerritoryQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Shop>().GetExistsAsync(y => y.TerritoryId == request.Id && y.IsActive) && !await unitOfWork.Repository<Entities.Models.Dealership>().GetExistsAsync(y => y.TerritoryId == request.Id && y.IsActive))
            {
                var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                territory.IsDelete = true;
                territory.IsActive = false;
                territory.ModifiedDate = DateTime.Now;
                territory.DeleteDate = DateTime.Now;
                territory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Territory>().Update(territory);
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
