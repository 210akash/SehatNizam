using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class GetAllAppointmentStatusHandler : IRequestHandler<GetAllAppointmentStatusQuery, List<GetAppointmentStatus>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllAppointmentStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAppointmentStatus>> Handle(GetAllAppointmentStatusQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.AppointmentStatus>().GetAllAsync();
            var order = mapper.Map<IEnumerable<GetAppointmentStatus>>(entity).ToList();
            return order;
        }
    }
}
