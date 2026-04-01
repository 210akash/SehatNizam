using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
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

        public GetPatientByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPatient>> Handle(GetPatientByNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.search))
                return new List<GetPatient>();

            var patients = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetAsync(x =>
                    string.IsNullOrEmpty(request.search) ||
                    EF.Functions.Like(x.Name, $"%{request.search}%") ||
                    EF.Functions.Like(x.PhoneNo, $"%{request.search}%") ||
                    EF.Functions.Like(x.MRN, $"%{request.search}%")
                );

            return mapper.Map<List<GetPatient>>(patients ?? new List<Entities.Models.Patient>());
        }
    }
}
