using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class GetSubCategoryByCompanyHandler : IRequestHandler<GetSubCategoryByCompanyQuery, List<GetSubCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        
        public GetSubCategoryByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetSubCategory>> Handle(GetSubCategoryByCompanyQuery request, CancellationToken cancellationToken)
        {
            if(request.CompanyId != 0)
            {
                var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _SubCategory = mapper.Map<List<GetSubCategory>>(SubCategory);
                return _SubCategory;
            }
            else
            {
                var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _SubCategory = mapper.Map<List<GetSubCategory>>(SubCategory);
                return _SubCategory;
            }
        
        }
    }
}
