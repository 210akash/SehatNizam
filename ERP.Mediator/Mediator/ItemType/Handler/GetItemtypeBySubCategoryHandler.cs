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
    public class GetItemtypeBySubCategoryHandler : IRequestHandler<GetItemtypeBySubCategoryQuery, List<GetItemType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemtypeBySubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetItemType>> Handle(GetItemtypeBySubCategoryQuery request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetAsync(y => y.SubCategoryId == request.SubCategoryId);
            var _ItemType = mapper.Map<List<GetItemType>>(ItemType);
            return _ItemType;
        }
    }
}
