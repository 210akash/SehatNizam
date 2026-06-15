using System.Collections.Generic;
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
    public class GetPatientByNameHandler : IRequestHandler<GetPatientByNameQuery, List<GetPatient>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPatientByNameHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetPatient>> Handle(GetPatientByNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Search))
                return new List<GetPatient>();

            var search = request.Search.Trim();

            var patients = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetAsync(
                    x => x.IsActive && !x.IsDelete
                         && x.PatientMaster != null
                         && (EF.Functions.Like(x.PatientMaster.Name, $"%{search}%")
                             || EF.Functions.Like(x.PatientMaster.PhoneNo, $"%{search}%")
                             || EF.Functions.Like(x.PatientMaster.CNIC, $"%{search}%")
                             || EF.Functions.Like(x.MRN, $"%{search}%")),
                    includeProperties: "PatientMaster,PatientMaster.City,Project");

            return mapper.Map<List<GetPatient>>(patients ?? new List<Entities.Models.Patient>());
        }
    }
}
