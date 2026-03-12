using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Handler
{
    public class GetShopTypeByNameHandler : IRequestHandler<GetShopTypeByNameQuery, List<GetShopType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopTypeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShopType>> Handle(GetShopTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var shopType = await unitOfWork.Repository<Entities.Models.ShopType>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _shopType = mapper.Map<List<GetShopType>>(shopType);
            return _shopType;
        }
    }
}
