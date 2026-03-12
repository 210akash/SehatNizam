using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using static ERP.Mediator.Mediator.SalesTarget.Handler.GetDSFTargetsByTerritoryIdHandler;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class GetDSFTargetsByTerritoryIdHandler : IRequestHandler<GetDSFTargetsByTerritoryIdQuery, List<UserTerritoryTargetDto>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDSFTargetsByTerritoryIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<UserTerritoryTargetDto>> Handle(GetDSFTargetsByTerritoryIdQuery request, CancellationToken cancellationToken)
        {
         

            var territoryUsrList = await unitOfWork.Repository<Entities.Models.UserTerritory>()
                .GetAsync(x => x.TerritoryId == request.TerritoryId && x.IsActive == true, null, null, "User.AspNetUserRoles");

            var UserTargetsByMonth = await unitOfWork.Repository<Entities.Models.SalesTarget>()
                .GetAsync(x => x.IsActive == true && x.UserId != null && x.TargetMonth.Year == request.TargetMonth.Year && x.TargetMonth.Month == request.TargetMonth.Month, null, null, null);

            var PreviousMonth = request.TargetMonth.AddMonths(-1);

            var UserPreviousTargetsByMonth = await unitOfWork.Repository<Entities.Models.SalesTarget>()
                .GetAsync(x => x.IsActive == true && x.UserId != null && x.TargetMonth.Year == PreviousMonth.Year && x.TargetMonth.Month == PreviousMonth.Month, null, null, null);

            var currentTargetByUserId = UserTargetsByMonth
                .GroupBy(x => x.UserId)
                .Select(g => g.FirstOrDefault())
                .ToDictionary(x => x.UserId, x => x);

            var previousTargetByUserId = UserPreviousTargetsByMonth
               .GroupBy(x => x.UserId)
               .Select(g => g.FirstOrDefault())
               .ToDictionary(x => x.UserId, x => x);


            var result = territoryUsrList.Select(user => new UserTerritoryTargetDto
            {
                UserId = user.UserId,
                UserName = user.User.FirstName + " " + user.User.LastName,
                Target = currentTargetByUserId.TryGetValue(user.UserId, out var target) ? target : null
            }).ToList();

            return result;
        }

        public class UserTerritoryTargetDto
        {
            public Guid? UserId { get; set; }
            public string UserName { get; set; }
            public Entities.Models.SalesTarget Target { get; set; }
        }
    }
}
