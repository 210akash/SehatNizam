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
    public class GetAccountSubCategoryByCategoryHandler : IRequestHandler<GetAccountSubCategoryByCategoryQuery, List<GetAccountSubCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountSubCategoryByCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountSubCategory>> Handle(GetAccountSubCategoryByCategoryQuery request, CancellationToken cancellationToken)
        {
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetAsync(y => y.AccountCategoryId == request.AccountCategoryId);
            var _AccountSubCategory = mapper.Map<List<GetAccountSubCategory>>(AccountSubCategory);
            return _AccountSubCategory;
        }
    }
}
