using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class GetAccountTypeByAccountSubCategoryHandler : IRequestHandler<GetAccountTypeByAccountSubCategoryQuery, List<GetAccountType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountTypeByAccountSubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountType>> Handle(GetAccountTypeByAccountSubCategoryQuery request, CancellationToken cancellationToken)
        {
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetAsync(y => y.AccountSubCategoryId == request.AccountSubCategoryId);
            var _AccountType = mapper.Map<List<GetAccountType>>(AccountType);
            return _AccountType;
        }
    }
}
