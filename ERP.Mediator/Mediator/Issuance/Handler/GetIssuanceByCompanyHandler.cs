using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetIssuanceByCompanyHandler : IRequestHandler<GetIssuanceByCompanyQuery, List<GetIssuance>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetIssuanceByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetIssuance>> Handle(GetIssuanceByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetAsync();
            var _Issuance = mapper.Map<List<GetIssuance>>(Issuance);
            return _Issuance;
        }
    }
}
