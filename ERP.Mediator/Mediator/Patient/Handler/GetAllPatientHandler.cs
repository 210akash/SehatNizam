using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Patient.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Patient.Handler
{
    public class GetAllPatientHandler : IRequestHandler<GetAllPatientQuery, Tuple<IEnumerable<GetPatient>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPatientHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPatient>, long>> Handle(GetAllPatientQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Patient, bool>> predicate = x =>
             x.IsActive &&
             (string.IsNullOrEmpty(request.Name) || EF.Functions.Like(x.Name, $"%{request.Name}%")) &&
             (string.IsNullOrEmpty(request.PhoneNo) || EF.Functions.Like(x.PhoneNo, $"%{request.PhoneNo}%")) &&
             (string.IsNullOrEmpty(request.MRN) || EF.Functions.Like(x.MRN, $"%{request.MRN}%")) &&
             (string.IsNullOrEmpty(request.CNIC) || EF.Functions.Like(x.CNIC, $"%{request.CNIC}%")) &&
             (x.ProjectId == sessionProvider.Session.SelectedWarehouseId) &&
             (request.CityId == null || x.CityId == request.CityId);

            Expression<Func<Entities.Models.Patient, object>>[] includes = {
                x => x.CreatedBy,
                x => x.City,
                x => x.Project,
                x => x.PatientAppointments
            };

            List<string> thenIncludes = new()
            {
                "PatientAppointments.Department",
                "PatientAppointments.Doctor",
                "PatientAppointments.Attachments",
                "PatientAppointments.LabOrders",
            };

            Expression<Func<Entities.Models.Patient, object>> OrderBy = null;
            Expression<Func<Entities.Models.Patient, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Patient>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Patient = mapper.Map<IEnumerable<GetPatient>>(entity.Item1).ToList();
            return new Tuple<IEnumerable<GetPatient>, long>(Patient, entity.Item2);
        }
    }
}
