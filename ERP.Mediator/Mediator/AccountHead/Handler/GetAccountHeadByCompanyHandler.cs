using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetAccountHeadByCompanyQuery, List<GetAccountHead>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountHead>> Handle(GetAccountHeadByCompanyQuery request, CancellationToken cancellationToken)
        {
            var AccountHead = await unitOfWork.Repository<Entities.Models.AccountHead>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _AccountHead = mapper.Map<List<GetAccountHead>>(AccountHead);
            return _AccountHead;
        }
    }
}
