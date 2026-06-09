using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Admission.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Admission.Handler
{
    public class GetAllAdmissionHandler : IRequestHandler<GetAllAdmissionQuery, Tuple<IEnumerable<GetAdmission>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAdmissionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAdmission>, long>> Handle(GetAllAdmissionQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;

            Expression<Func<Entities.Models.Admission, object>>[] includes = {
                x => x.Status,
                x => x.CreatedBy,
                x => x.AdmissionPackageMaster,
                x => x.AdmissionPackageMaster.AdmissionPackageDetail,
                x => x.AdmissionBeds.Where(y=>y.IsActive),
                x => x.Appointment.Project,
                x => x.Appointment.Patient,
                x => x.Appointment.Patient.PatientMaster,
                x => x.Appointment.Doctor,
                x => x.Appointment.Department,
                x => x.Appointment.Department.Company,
                x => x.Appointment.PriorityLevel,
                x => x.Appointment.AppointmentType,
                x => x.Appointment.VisitType,
                x => x.Appointment.AppointmentStatus,
                x => x.Appointment.Referrer,
                x => x.Appointment.AppointmentPayments
            };

            List<string> thenIncludes = new()
            {
                "AdmissionPackageMaster.AdmissionPackageDetail.Service",
                "Appointment.AppointmentPayments.PaymentMode",
                "Appointment.AppointmentPayments.PaymentStatus",
                "AdmissionBeds.Bed",
                "AdmissionBeds.Bed.Room",
                "AdmissionBeds.Bed.Room.Ward",

            };

            Expression<Func<Entities.Models.Admission, bool>> predicate =
             x => x.IsActive == true
             && (request.Id == null || x.Id == request.Id.Value)
             && x.Appointment.ProjectId == sessionProvider.Session.SelectedWarehouseId
             && x.AdmissionDate >= request.FDate.Date
             && x.AdmissionDate <= request.TDate.Date.AddDays(1).AddTicks(-1)
             && (request.TokenNo == null || request.TokenNo == "" || x.Appointment.TokenNumber.Contains(request.TokenNo))
             && (request.MRN == null || request.MRN == "" || x.Appointment.Patient.MRN.Contains(request.MRN))
             && (request.PatientName == null || request.PatientName == "" || x.Appointment.Patient.PatientMaster.Name.ToLower().Trim().Contains(request.PatientName.ToLower().Trim()))
             && (request.StatusId == null || x.StatusId == request.StatusId.Value);
            // && (request.DepartmentId == null || x.DepartmentId == request.DepartmentId.Value)
            // && (request.BookingFormType == 1 || request.BookingFormType == 5 && x.AppointmentStatusId != 1);

            Expression<Func<Entities.Models.Admission, object>> OrderBy = null;
            Expression<Func<Entities.Models.Admission, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Admission>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Appointment = mapper.Map<IEnumerable<GetAdmission>>(entity.Item1).ToList();
            return new Tuple<IEnumerable<GetAdmission>, long>(Appointment, entity.Item2);
        }
    }
}
