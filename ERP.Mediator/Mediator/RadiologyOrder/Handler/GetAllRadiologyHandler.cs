using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class GetAllRadiologyOrderHandler : IRequestHandler<GetAllRadiologyOrderQuery, Tuple<IEnumerable<GetRadiologyOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllRadiologyOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRadiologyOrder>, long>> Handle(GetAllRadiologyOrderQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.RadiologyOrder, bool>> predicate =
                x => x.IsActive == true
                && x.CreatedDate >= request.FDate
                && x.CreatedDate <= request.TDate.AddDays(1).AddTicks(-1)
                && (request.TokenNo == "" || x.Appointment.TokenNumber.Contains(request.TokenNo))
                && (request.MRN == "" || x.Appointment.Patient.MRN.Contains(request.MRN))
                && (request.Name == "" || x.Appointment.Patient.PatientMaster.Name.ToLower().Trim().Contains(request.Name.ToLower().Trim()))
                && (!request.RadiologyTypeId.HasValue || x.RadiologyTypeId == request.RadiologyTypeId.Value)
                && (!request.StatusId.HasValue || x.StatusId == request.StatusId.Value);

            Expression<Func<Entities.Models.RadiologyOrder, object>>[] includes = { x => x.Status, x => x.RadiologyType , x => x.Appointment, x => x.Appointment.Patient, x => x.Appointment.Patient.PatientMaster, x => x.Appointment.Department, x => x.Appointment.Department.Company, x => x.Appointment.Referrer, x => x.RadiologyType.Service, x => x.RadiologyStudyResult ,x => x.RadiologyStudyResult.Images };
            var result = unitOfWork.Repository<Entities.Models.RadiologyOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, x => x.Id, null, includes);
            return new Tuple<IEnumerable<GetRadiologyOrder>, long>(mapper.Map<IEnumerable<GetRadiologyOrder>>(result.Item1), result.Item2);
        }
    }
}
