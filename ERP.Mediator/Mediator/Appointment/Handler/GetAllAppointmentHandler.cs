using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class GetAllAppointmentHandler : IRequestHandler<GetAllAppointmentQuery, Tuple<IEnumerable<GetAppointment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAppointmentHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAppointment>, long>> Handle(GetAllAppointmentQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;

            Expression<Func<Entities.Models.Appointment, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Project,
                x => x.Patient,
                x => x.Doctor,
                x => x.Department,
                x => x.Department.Company,
                x => x.PriorityLevel,
                x => x.AppointmentType,
                x => x.VisitType,
                x => x.AppointmentStatus,
                x => x.Referrer,
                x => x.AppointmentPayments
            };

            List<string> thenIncludes = new()
            {
                "AppointmentPayments.PaymentMode",
                "AppointmentPayments.PaymentStatus",
            };

            Expression<Func<Entities.Models.Appointment, bool>> predicate =
             x => x.IsActive == true
             && x.DoctorId != null
             && (request.Id == null || x.Id == request.Id.Value)
             && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
             && x.AppointmentDate >= request.FDate.Date
             && x.AppointmentDate <= request.TDate.Date.AddDays(1).AddTicks(-1)
             && (request.TokenNo == null || request.TokenNo == "" || x.TokenNumber.Contains(request.TokenNo))
             && (request.MRN == null || request.MRN == "" || x.Patient.MRN.Contains(request.MRN))
             && (request.PatientName == null || request.PatientName == "" || x.Patient.Name.ToLower().Trim().Contains(request.PatientName.ToLower().Trim()))
             && (request.StatusId == null || x.AppointmentStatusId == request.StatusId.Value)
             && (request.DepartmentId == null || x.DepartmentId == request.DepartmentId.Value)
             && (request.BookingFormType == 1 || request.BookingFormType == 5 && x.AppointmentStatusId != 1);

            Expression<Func<Entities.Models.Appointment, object>> OrderBy = null;
            Expression<Func<Entities.Models.Appointment, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Appointment>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Appointment = mapper.Map<IEnumerable<GetAppointment>>(entity.Item1).ToList();
            return new Tuple<IEnumerable<GetAppointment>, long>(Appointment, entity.Item2);
        }
    }
}
