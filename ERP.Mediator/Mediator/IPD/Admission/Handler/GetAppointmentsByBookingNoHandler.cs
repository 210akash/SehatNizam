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
    public class GetAppointmentsByBookingNoHandler : IRequestHandler<GetAppointmentsByBookingNoQuery, List<GetAppointment>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAppointmentsByBookingNoHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAppointment>> Handle(GetAppointmentsByBookingNoQuery request, CancellationToken cancellationToken)
        {
            // Get top 10 appointments where BookingNoNumber contains the requested BookingNo
            var appointments = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetAsync(
                    filter: y => y.Id.ToString().Contains(request.BookingNo),
                    orderBy: q => q.OrderBy(a => a.Id),  // ascending order
                    includeProperties: "Patient,Doctor,Department,Project",
                    take: 5
                );

            // Map the result to DTOs
            var mappedAppointments = mapper.Map<List<GetAppointment>>(appointments);
            return mappedAppointments;
        }
    }
}
