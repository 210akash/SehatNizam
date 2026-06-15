using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AdvancePayments.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Handler
{
    public class GetAllAdvancePaymentsHandler : IRequestHandler<GetAllAdvancePaymentsQuery, Tuple<IEnumerable<GetAdvancePayment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAdvancePaymentsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAdvancePayment>, long>> Handle(GetAllAdvancePaymentsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AdvancePayment, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
            && (string.IsNullOrEmpty(request.AppointmentNo) || x.Appointment.TokenNumber.Contains(request.AppointmentNo))
            && (x.Appointment.ProjectId == sessionProvider.Session.SelectedWarehouseId)
             && x.PaymentDate >= request.FDate.Date
             && x.PaymentDate <= request.TDate.Date.AddDays(1).AddTicks(-1)
             && (request.AppointmentNo == null || request.AppointmentNo == "" || x.Appointment.TokenNumber.Contains(request.AppointmentNo))
             && (request.MRN == null || request.MRN == "" || x.Appointment.Patient.MRN.Contains(request.MRN))
             && (request.PatientName == null || request.PatientName == "" || x.Appointment.Patient.PatientMaster.Name.ToLower().Trim().Contains(request.PatientName.ToLower().Trim()))
             && (request.StatusId == null || x.PaymentStatusId == request.StatusId.Value);

            Expression<Func<Entities.Models.AdvancePayment, object>>[] includes = {
                x => x.Appointment,
                x => x.Appointment.Patient,
                x => x.Appointment.Patient.PatientMaster,
                x => x.PaymentMode,
                x => x.PaymentStatus
            };

            Expression<Func<Entities.Models.AdvancePayment, object>> OrderBy = null;
            Expression<Func<Entities.Models.AdvancePayment, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.AdvancePayment>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Project = mapper.Map<IEnumerable<GetAdvancePayment>>(entity.Item1.ToList());
            return new Tuple<IEnumerable<GetAdvancePayment>, long>(Project, entity.Item2);
        }
    }
}
