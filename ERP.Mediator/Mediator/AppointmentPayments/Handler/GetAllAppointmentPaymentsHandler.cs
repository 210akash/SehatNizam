using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentPayments.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentPayments.Handler
{
    public class GetAllAppointmentPaymentsHandler : IRequestHandler<GetAllAppointmentPaymentsQuery, Tuple<IEnumerable<GetAppointmentPayment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAppointmentPaymentsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAppointmentPayment>, long>> Handle(GetAllAppointmentPaymentsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AppointmentPayment, bool>> predicate =
                x => x.IsActive == true
                && x.IsDelete == false
                && x.Appointment.ProjectId == sessionProvider.Session.SelectedWarehouseId
                && (request.AppointmentId != null || (x.CreatedDate != null && x.CreatedDate >= request.FDate.Date && x.CreatedDate <= request.TDate.Date.AddDays(1).AddTicks(-1)))
                && (request.AppointmentId == null || x.AppointmentId == request.AppointmentId.Value)
                && (request.TokenNo == null || request.TokenNo == "" || x.Appointment.TokenNumber.Contains(request.TokenNo))
                && (request.MRN == null || request.MRN == "" || x.Appointment.Patient.MRN.Contains(request.MRN))
                && (request.PatientName == null || request.PatientName == "" || x.Appointment.Patient.PatientMaster.Name.ToLower().Trim().Contains(request.PatientName.ToLower().Trim()))
                && (request.PaymentStatusId == null
                    || (request.PaymentStatusId == -1 && (x.PaymentStatusId == 1 || x.PaymentStatusId == 2))
                    || (request.PaymentStatusId != -1 && x.PaymentStatusId == request.PaymentStatusId.Value))
                && (request.PaymentModeId == null || x.PaymentModeId == request.PaymentModeId.Value)
                && (request.ServiceId == null || x.ServiceId == request.ServiceId.Value)
                && (request.ServiceIds == null || request.ServiceIds.Count == 0 || request.ServiceIds.Contains(x.ServiceId));

            Expression<Func<Entities.Models.AppointmentPayment, object>>[] includes =
            {
                x => x.Appointment,
                x => x.Service,
                x => x.PaymentMode,
                x => x.PaymentStatus
            };

            List<string> thenIncludes = new()
            {
                "Appointment.Patient",
                "Appointment.Patient.PatientMaster",
                "Appointment.Department",
                "Appointment.Doctor"
            };

            Expression<Func<Entities.Models.AppointmentPayment, object>> orderByDesc = x => x.CreatedDate;
            var entity = unitOfWork.Repository<Entities.Models.AppointmentPayment>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, thenIncludes, includes);

            var result = mapper.Map<IEnumerable<GetAppointmentPayment>>(entity.Item1.ToList());
            return new Tuple<IEnumerable<GetAppointmentPayment>, long>(result, entity.Item2);
        }
    }
}
