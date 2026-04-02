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
            Expression<Func<Entities.Models.Appointment, bool>> predicate;

            Expression<Func<Entities.Models.Appointment, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Project,
                x => x.Patient,
                x => x.Doctor,
                x => x.Department,
                x => x.PriorityLevel,
                x => x.AppointmentType,
                x => x.VisitType,
                x => x.AppointmentStatus,
                x => x.AppointmentPayments
            };

            List<string> thenIncludes = new()
            {
                "AppointmentPayments.PaymentMode",
                "AppointmentPayments.PaymentStatus",
            };

            predicate = x => x.IsActive == true;
            //// Check if the current user's RoleId array contains the AccountOwnerRoleId
            //if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            //{
            //    predicate = x => x.IsActive == true
            //    &&(request.Name == "" || request.Name == null || x.Name == request.Name)
            //    && x.CompanyId == this.sessionProvider.Session.CompanyId;
            //}
            //else
            //{
            //    predicate = x => x.IsActive == true
            //      && (request.Name == "" || request.Name == null || x.Name == request.Name);
            //}

            Expression<Func<Entities.Models.Appointment, object>> OrderBy = null;
            Expression<Func<Entities.Models.Appointment, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Appointment>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var data = (IEnumerable<Entities.Models.Appointment>)entity.Item1;

            var Appointment = mapper.Map<IEnumerable<GetAppointment>>(data).ToList();
            return new Tuple<IEnumerable<GetAppointment>, long>(Appointment, entity.Item2);
        }
    }
}
