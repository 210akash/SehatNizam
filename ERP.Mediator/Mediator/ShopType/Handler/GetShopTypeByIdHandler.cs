using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Handler
{
    public class GetShopTypeByIdHandler : IRequestHandler<GetShopTypeByIdQuery, GetShopType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetShopType> Handle(GetShopTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var shopType = await unitOfWork.Repository<Entities.Models.ShopType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _shopType = mapper.Map<GetShopType>(shopType);
            return _shopType;
        }
    }
}
