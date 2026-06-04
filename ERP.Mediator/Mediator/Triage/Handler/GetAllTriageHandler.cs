using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Triage.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Triage.Handler
{
    public class GetAllTriageHandler : IRequestHandler<GetAllTriageQuery, Tuple<IEnumerable<GetTriage>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllTriageHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }



        public async Task<Tuple<IEnumerable<GetTriage>, long>> Handle(
      GetAllTriageQuery request,
      CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Triage, bool>> predicate = x =>
                x.IsActive == true
                && (request.BookingNo == "" || x.Appointment.Patient.Id.ToString().Contains(request.BookingNo))
                && (request.Name == "" || x.Appointment.Patient.Name.ToLower().Contains(request.Name.ToLower()));

            Expression<Func<Entities.Models.Triage, object>>[] includes =
            {
        x => x.Appointment,
        x => x.Appointment.Doctor,
        x => x.Appointment.Department,
        x => x.Appointment.Department.Company,
    x => x.Appointment.Patient,
    x => x.Nurse,
    x => x.SugarType,
    x => x.TriagePriority,
    };

            Expression<Func<Entities.Models.Triage, object>> OrderBy = null;
            Expression<Func<Entities.Models.Triage, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Triage>()
                .GetPagingWhereAsNoTrackingAsync(
                    predicate,
                    request.PagingData,
                    OrderBy,
                    OrderByDesc,
                    null,
                    includes);

            // ... your existing code to get entity ...
            var triageList = entity.Item1.ToList();
            var triage = mapper.Map<List<GetTriage>>(triageList);
            return new Tuple<IEnumerable<GetTriage>, long>(triage, entity.Item2);
        }
    }
}
