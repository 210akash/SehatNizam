using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Handler
{
    public class GetAccountTypeByCompanyHandler : IRequestHandler<GetAccountTypeByCompanyQuery, List<GetAccountType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountTypeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountType>> Handle(GetAccountTypeByCompanyQuery request, CancellationToken cancellationToken)
        {
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _AccountType = mapper.Map<List<GetAccountType>>(AccountType);
            return _AccountType;
        }
    }
}
