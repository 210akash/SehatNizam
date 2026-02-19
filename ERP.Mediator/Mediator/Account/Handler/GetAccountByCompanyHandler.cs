using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetAccountByCompanyHandler : IRequestHandler<GetAccountByCompanyQuery, List<GetAccount>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccount>> Handle(GetAccountByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Account = mapper.Map<List<GetAccount>>(Account);
            return _Account;
        }
    }
}
