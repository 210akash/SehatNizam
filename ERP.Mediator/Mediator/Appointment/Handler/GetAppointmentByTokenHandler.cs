using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class GetAppointmentByTokenHandler : IRequestHandler<GetAppointmentByTokenQuery, List<GetAppointment>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAppointmentByTokenHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAppointment>> Handle(GetAppointmentByTokenQuery request, CancellationToken cancellationToken)
        {
            // Get top 10 appointments where TokenNumber contains the requested token
            var appointments = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetAsync(
                    filter: y => y.TokenNumber.Contains(request.Token) && (request.StatusId == null || y.AppointmentStatusId == request.StatusId),
                    orderBy: q => q.OrderBy(a => a.TokenNumber),  // ascending order
                    includeProperties: "Patient,Doctor,Department,Project",
                    take: 5
                );

            // Map the result to DTOs
            var mappedAppointments = mapper.Map<List<GetAppointment>>(appointments);
            return mappedAppointments;
        }
    }
}
