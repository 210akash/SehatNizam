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
    public class GetAllAppointmentByDoctorHandler : IRequestHandler<GetAllAppointmentByDoctorQuery, Tuple<IEnumerable<GetAppointment>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllAppointmentByDoctorHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetAppointment>, long>> Handle(GetAllAppointmentByDoctorQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Appointment, bool>> predicate;

            Expression<Func<Entities.Models.Appointment, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Project,
                x => x.Patient,
                x => x.Problems,
                x => x.Doctor,
                x => x.Department,
                x => x.PriorityLevel,
                x => x.AppointmentType,
                x => x.VisitType,
                x => x.Triages.Where(y=>y.IsActive),
                x => x.AppointmentStatus,
            };

            List<string> thenIncludes = new()
            {
                "Problems.Status",
            };

            predicate = x => x.IsActive == true
           // && x.DoctorId == sessionProvider.Session.LoggedInUserId
           // && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                      && x.AppointmentStatusId == request.StatusId;
                      //&& x.CreatedDate >= request.FDate.Value
                      //&& x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1);

            Expression<Func<Entities.Models.Appointment, object>> OrderBy = null;
            Expression<Func<Entities.Models.Appointment, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Appointment>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Appointment = mapper.Map<IEnumerable<GetAppointment>>(entity.Item1).ToList();
            return new Tuple<IEnumerable<GetAppointment>, long>(Appointment, entity.Item2);
        }
    }
}
