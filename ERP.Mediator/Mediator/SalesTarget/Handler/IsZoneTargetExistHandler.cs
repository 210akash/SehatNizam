using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SalesTarget.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SalesTarget.Handler
{
    public class IsZoneTargetExistHandler : IRequestHandler<IsZoneTargetExistQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public IsZoneTargetExistHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<bool> Handle(IsZoneTargetExistQuery request, CancellationToken cancellationToken)
        {
            var isZoneTargetExist = await unitOfWork.Repository<Entities.Models.SalesTarget>().GetExistsAsync(y => y.Id == request.ZoneId && y.TargetMonth.Month == DateTime.Now.Month);
            return isZoneTargetExist;
        }
    }
}
