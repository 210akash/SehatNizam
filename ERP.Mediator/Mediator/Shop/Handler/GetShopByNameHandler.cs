using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetShopByNameHandler : IRequestHandler<GetShopByNameQuery, List<GetShop>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShop>> Handle(GetShopByNameQuery request, CancellationToken cancellationToken)
        {
            var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetAsync(y => y.IsActive == true && y.Name.ToLower().Contains(request.name));
            var _shop = mapper.Map<List<GetShop>>(shop);
            return _shop;
        }
    }
}
