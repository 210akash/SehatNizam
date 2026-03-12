using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserAttendance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetUserAttendanceByNameHandler : IRequestHandler<GetUserAttendanceByNameQuery, List<GetUserAttendance>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetUserAttendanceByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetUserAttendance>> Handle(GetUserAttendanceByNameQuery request, CancellationToken cancellationToken)
        {
            var UserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetAsync(y => y.IsActive);
            var _UserAttendance = mapper.Map<List<GetUserAttendance>>(UserAttendance);
            return _UserAttendance;
        }
    }
}
