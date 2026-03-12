using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UserTerritory.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class SaveUserTerritoryHandler : IRequestHandler<SaveUserTerritoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveUserTerritoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveUserTerritoryCommand, long>.Handle(SaveUserTerritoryCommand request, CancellationToken cancellationToken)
        {
            bool existingUserTerritory = false;

            var currentRole = unitOfWork.Repository<AspNetUserRoles>().GetFirstAsNoTrackingAsync(x => x.UserId == request.UserId, null, null, "Role").Result.Role.Name;

            if (currentRole == "ASE" || currentRole == "ASD" || currentRole == "KSS" || currentRole == "Distributor" || currentRole == "DSF" || currentRole == "Retailer")
            {
                existingUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetExistsAsync(x => x.UserId == request.UserId && x.TerritoryId == request.TerritoryId && x.IsActive == true);
            }
            else if (currentRole == "ZSM")
            {
                existingUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetExistsAsync(x => x.UserId == request.UserId && x.ZoneId == request.ZoneId && x.IsActive == true);
            }
            else if (currentRole == "RSM")
            {
                existingUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetExistsAsync(x => x.UserId == request.UserId && x.RegionId == request.RegionId && x.IsActive == true);
            }
            else if (currentRole == "ASM")
            {
                existingUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetExistsAsync(x => x.UserId == request.UserId && x.AreaId == request.AreaId && x.IsActive == true);
            }
            
            if (existingUserTerritory == false)
            {
                var _UserTerritory_master = mapper.Map<Entities.Models.UserTerritory>(request);
                _UserTerritory_master.CreatedById = sessionProvider.Session.LoggedInUserId;
                _UserTerritory_master.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.UserTerritory>().Add(_UserTerritory_master);
                SaveChanges();
                return 200;
            }
            else
            {
                return 409;
            }


        }
    }
}