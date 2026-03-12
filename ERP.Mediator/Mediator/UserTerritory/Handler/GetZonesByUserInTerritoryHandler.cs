using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.UserTerritory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetZonesByUserInTerritoryHandler : IRequestHandler<GetZonesByUserInTerritoryQuery, List<GetZone>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetZonesByUserInTerritoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetZone>> Handle(GetZonesByUserInTerritoryQuery request, CancellationToken cancellationToken)
        {
            var getUserRole = unitOfWork.Repository<AspNetUserRoles>().GetAsync(x => x.UserId == request.UserId, null, null, "Role").Result.FirstOrDefault().Role.Name;
            var userTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(x => x.UserId == request.UserId && x.IsActive == true);

            List<GetZone> _zoneList = new List<GetZone>();

            if(getUserRole != "ZSM")
            {
                if (userTerritory == null)
                {
                    var allZones = await unitOfWork.Repository<Entities.Models.Zone>().GetAllAsync();
                    _zoneList = mapper.Map<List<GetZone>>(allZones);
                    return _zoneList;
                }
                else
                {
                    var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetFirstAsNoTrackingAsync(x => x.Id == userTerritory.ZoneId);
                    var _zone = mapper.Map<GetZone>(zone);
                    _zoneList.Add(_zone);
                    return _zoneList;
                }
            }
            else
            {
                var allZones = await unitOfWork.Repository<Entities.Models.Zone>().GetAllAsync();
                _zoneList = mapper.Map<List<GetZone>>(allZones);
                return _zoneList;
            }
        }
    }
}
