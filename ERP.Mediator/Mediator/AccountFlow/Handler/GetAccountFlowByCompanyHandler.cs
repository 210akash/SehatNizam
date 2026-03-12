using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountFlow.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetAccountFlowByCompanyQuery, List<GetAccountFlow>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountFlow>> Handle(GetAccountFlowByCompanyQuery request, CancellationToken cancellationToken)
        {
            var AccountFlow = await unitOfWork.Repository<Entities.Models.AccountFlow>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _AccountFlow = mapper.Map<List<GetAccountFlow>>(AccountFlow);
            return _AccountFlow;
        }
    }
}
