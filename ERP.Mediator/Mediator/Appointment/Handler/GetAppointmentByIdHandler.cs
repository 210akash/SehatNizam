using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class GetAppointmentByIdHandler : IRequestHandler<GetAppoinmentByIdQuery, GetAppointment>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAppointmentByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetAppointment> Handle(GetAppoinmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id,
                null,null, "Project,Patient,Doctor,Department,PriorityLevel,AppointmentType,Attachments,Triages");
            var _appointment = mapper.Map<GetAppointment>(appointment);
            return _appointment;
        }
    }
}
