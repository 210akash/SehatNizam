using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.UserAttendance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UserTerritory.Handler
{
    public class GetUserAttendanceByIdHandler : IRequestHandler<GetUserAttendanceByIdQuery, GetUserAttendance>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetUserAttendanceByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetUserAttendance> Handle(GetUserAttendanceByIdQuery request, CancellationToken cancellationToken)
        {
            var UserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _UserAttendance = mapper.Map<GetUserAttendance>(UserAttendance);
            return _UserAttendance;
        }
    }
}
