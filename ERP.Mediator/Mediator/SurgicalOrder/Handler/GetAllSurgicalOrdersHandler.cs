using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SurgicalOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.SurgicalOrder.Handler
{
    public class GetAllSurgicalOrdersHandler : IRequestHandler<GetAllSurgicalOrdersQuery, Tuple<IEnumerable<GetSurgicalOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllSurgicalOrdersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetSurgicalOrder>, long>> Handle(GetAllSurgicalOrdersQuery request, CancellationToken cancellationToken)
        {
            var filterByAppointment = request.AppointmentId.HasValue && request.AppointmentId.Value > 0;
            var fDate = request.FDate ?? DateTime.MinValue;
            var tDate = GetEndOfDay(request.TDate);
            var tokenNo = request.TokenNo ?? string.Empty;
            var name = request.Name ?? string.Empty;

            Expression<Func<Entities.Models.SurgicalOrder, bool>> predicate =
                x => !x.IsDelete
                && x.IsActive
                && (filterByAppointment || (x.ScheduledDateTime >= fDate && x.ScheduledDateTime <= tDate))
                && (!request.AppointmentId.HasValue || x.AppointmentId == request.AppointmentId.Value)
                && (!request.SurgeonId.HasValue || x.SurgeonId == request.SurgeonId.Value)
                && (!request.ServiceId.HasValue || x.ServiceId == request.ServiceId.Value)
                && (!request.StatusId.HasValue || x.StatusId == request.StatusId.Value)
                && (tokenNo == string.Empty || (x.Appointment.TokenNumber != null && x.Appointment.TokenNumber.Contains(tokenNo)))
                && (name == string.Empty || (x.Appointment.Patient.PatientMaster.Name != null && x.Appointment.Patient.PatientMaster.Name.ToLower().Contains(name.ToLower())));

            Expression<Func<Entities.Models.SurgicalOrder, object>>[] includes =
            {
                x => x.Status,
                x => x.Service,
                x => x.Surgeon,
                x => x.Appointment,
                x => x.Appointment.Patient,
                x => x.Appointment.Patient.PatientMaster,
                x => x.Appointment.Department,
                x => x.Appointment.Doctor
            };

            var result = unitOfWork.Repository<Entities.Models.SurgicalOrder>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, x => x.ScheduledDateTime, null, includes);

            return new Tuple<IEnumerable<GetSurgicalOrder>, long>(
                mapper.Map<IEnumerable<GetSurgicalOrder>>(result.Item1.ToList()),
                result.Item2);
        }

        private static DateTime GetEndOfDay(DateTime? date)
        {
            if (!date.HasValue)
                return DateTime.MaxValue;

            var day = date.Value.Date;
            if (day >= DateTime.MaxValue.Date)
                return DateTime.MaxValue;

            return day.AddDays(1).AddTicks(-1);
        }
    }
}
