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
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.AppointmentPayments.Handler
{
    public class GetAppointmentPaymentGroupsHandler : IRequestHandler<GetAppointmentPaymentGroupsQuery, Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAppointmentPaymentGroupsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>> Handle(GetAppointmentPaymentGroupsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.AppointmentPayment, bool>> predicate =
                x => x.IsActive == true
                && x.IsDelete == false
                && x.Appointment.ProjectId == sessionProvider.Session.SelectedWarehouseId
                && x.CreatedDate != null
                && x.CreatedDate >= request.FDate.Date
                && x.CreatedDate <= request.TDate.Date.AddDays(1).AddTicks(-1)
                && (request.TokenNo == null || request.TokenNo == "" || x.Appointment.TokenNumber.Contains(request.TokenNo))
                && (request.MRN == null || request.MRN == "" || x.Appointment.Patient.MRN.Contains(request.MRN))
                && (request.PatientName == null || request.PatientName == "" || x.Appointment.Patient.PatientMaster.Name.ToLower().Trim().Contains(request.PatientName.ToLower().Trim()))
                && (request.PaymentStatusId == null
                    || (request.PaymentStatusId == -1 && (x.PaymentStatusId == 1 || x.PaymentStatusId == 2))
                    || (request.PaymentStatusId != -1 && x.PaymentStatusId == request.PaymentStatusId.Value))
                && (request.PaymentModeId == null || x.PaymentModeId == request.PaymentModeId.Value)
                && (request.ServiceId == null || x.ServiceId == request.ServiceId.Value)
                && (request.ServiceIds == null || request.ServiceIds.Count == 0 || request.ServiceIds.Contains(x.ServiceId));

            var groupedQuery = unitOfWork.Repository<Entities.Models.AppointmentPayment>().Entities
                .AsNoTracking()
                .Where(predicate)
                .GroupBy(x => x.AppointmentId)
                .Select(g => new GetAppointmentPaymentGroup
                {
                    AppointmentId = g.Key,
                    PendingPaymentCount = g.Count(x => x.PaymentStatusId == 1 || x.PaymentStatusId == 2),
                    ApprovedPaymentCount = g.Count(x => x.PaymentStatusId == 3),
                    PendingGrandTotal = g.Where(x => x.PaymentStatusId == 1 || x.PaymentStatusId == 2).Sum(x => x.TotalPayable),
                    ApprovedGrandTotal = g.Where(x => x.PaymentStatusId == 3).Sum(x => x.TotalPayable),
                    TotalVisitFee = g.Sum(x => x.VisitFee),
                    TotalDiscount = g.Sum(x => x.Discount),
                    GrandTotal = g.Sum(x => x.TotalPayable),
                    LastCreatedDate = g.Max(x => x.CreatedDate)
                })
                .OrderByDescending(x => x.LastCreatedDate);

            var total = await groupedQuery.CountAsync(cancellationToken);

            IQueryable<GetAppointmentPaymentGroup> pagedQuery = groupedQuery;
            if (request.PagingData != null && request.PagingData.IsPagingEnabled)
            {
                pagedQuery = groupedQuery
                    .Skip(request.PagingData.Skip)
                    .Take(request.PagingData.Take);
            }

            var groups = await pagedQuery.ToListAsync(cancellationToken);
            if (groups.Count == 0)
            {
                return new Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>(groups, total);
            }

            var appointmentIds = groups.Select(x => x.AppointmentId).ToList();

            var appointments = await unitOfWork.Repository<Entities.Models.Appointment>().Entities
                .AsNoTracking()
                .Where(x => appointmentIds.Contains(x.Id))
                .Include(x => x.Patient)
                .ThenInclude(p => p.PatientMaster)
                .Include(x => x.Department)
                .Include(x => x.Doctor)
                .ToListAsync(cancellationToken);

            var appointmentMap = mapper.Map<List<GetAppointment>>(appointments)
                .ToDictionary(x => x.Id);

            foreach (var group in groups)
            {
                if (appointmentMap.TryGetValue(group.AppointmentId, out var appointment))
                {
                    group.Appointment = appointment;
                }
            }

            return new Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>(groups, total);
        }
    }
}
