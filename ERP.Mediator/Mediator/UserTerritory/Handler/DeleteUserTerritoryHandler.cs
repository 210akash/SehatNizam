using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class DeleteUserTerritoryHandler : IRequestHandler<DeleteUserTerritoryQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteUserTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteUserTerritoryQuery request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.Repository<Entities.Models.UserTerritory>().GetExistsAsync(y => y.Id == request.Id))
            {
                var UserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                UserTerritory.IsDelete = true;
                UserTerritory.IsActive = false;
                UserTerritory.ModifiedDate = DateTime.Now;
                UserTerritory.DeleteDate = DateTime.Now;
                UserTerritory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.UserTerritory>().Update(UserTerritory);
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
