using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class GetAppointmentByTokenHandler : IRequestHandler<GetAppointmentByTokenQuery, GetAppointment>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAppointmentByTokenHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetAppointment> Handle(GetAppointmentByTokenQuery request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>().GetFirstAsNoTrackingAsync(y => y.TokenNumber == request.Token,null, null, "Patient,Doctor,Department,Project");
            var _appointment = mapper.Map<GetAppointment>(appointment);
            return _appointment;
        }
    }
}
