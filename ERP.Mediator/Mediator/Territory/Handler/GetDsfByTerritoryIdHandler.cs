using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Territory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class GetDsfByTerritoryIdHandler : IRequestHandler<GetDsfByTerritoryIdQuery, List<GetUsers>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDsfByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetUsers>> Handle(GetDsfByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
            var dsfRole = unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "DSF").Result.Id;
            var salesmanRole = unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "Salesman").Result.Id;

            var _dsf = new GetUsers();
            List<GetUsers> getUsersByTerritory = new List<GetUsers>();

            var dsfList = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetAsync(x => x.TerritoryId == request.TerritoryId && x.IsActive == true);
            foreach (var item in dsfList)
            {
                var dsf = await unitOfWork.Repository<AspNetUsers>().GetFirstAsNoTrackingAsync(x => x.Id == item.UserId, null, null, "AspNetUserRoles,UserTerritory");
                if(dsf.AspNetUserRoles.Any(x => x.RoleId == dsfRole || x.RoleId == salesmanRole))
                {
                    _dsf = mapper.Map<GetUsers>(dsf);
                    getUsersByTerritory.Add(_dsf);
                }
            }

            return getUsersByTerritory;
        }
    }
}
