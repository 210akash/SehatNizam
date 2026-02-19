using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class GetAccountSubCategoryByCompanyHandler : IRequestHandler<GetAccountSubCategoryByCompanyQuery, List<GetAccountSubCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        
        public GetAccountSubCategoryByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetAccountSubCategory>> Handle(GetAccountSubCategoryByCompanyQuery request, CancellationToken cancellationToken)
        {
            if(request.CompanyId != 0)
            {
                var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _AccountSubCategory = mapper.Map<List<GetAccountSubCategory>>(AccountSubCategory);
                return _AccountSubCategory;
            }
            else
            {
                var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _AccountSubCategory = mapper.Map<List<GetAccountSubCategory>>(AccountSubCategory);
                return _AccountSubCategory;
            }
        
        }
    }
}
