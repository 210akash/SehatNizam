using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetShopByIdHandler : IRequestHandler<GetShopByIdQuery, GetShop>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetShop> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
        {
            var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(y => y.IsActive == true && y.Id == request.Id);
            var _shop = mapper.Map<GetShop>(shop);
            return _shop;
        }
    }
}
