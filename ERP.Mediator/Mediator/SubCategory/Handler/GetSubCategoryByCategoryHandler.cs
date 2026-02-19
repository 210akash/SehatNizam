using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class GetSubCategoryByCategoryHandler : IRequestHandler<GetSubCategoryByCategoryQuery, List<GetSubCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetSubCategoryByCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetSubCategory>> Handle(GetSubCategoryByCategoryQuery request, CancellationToken cancellationToken)
        {
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetAsync(y => y.CategoryId == request.CategoryId);
            var _SubCategory = mapper.Map<List<GetSubCategory>>(SubCategory);
            return _SubCategory;
        }
    }
}
