using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Twilio.TwiML.Voice;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class GetShopsByRouteIdHandler : IRequestHandler<GetShopsByRouteIdQuery, List<GetShop>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetShopsByRouteIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetShop>> Handle(GetShopsByRouteIdQuery request, CancellationToken cancellationToken)
        {
            List<GetShop> getshopsByRoute = new List<GetShop>();

            var routeShops = await unitOfWork.Repository<ShopRouteFrequency>().GetAsync(y => y.RouteId == request.RouteId && y.IsActive == true, null, null, "Shop");
            foreach (var item in routeShops)
            {
                var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == item.ShopId && x.IsActive == true);
                var _shop = mapper.Map<GetShop>(shop);
                if(_shop != null)
                {
                    getshopsByRoute.Add(_shop);
                }
            }

            return getshopsByRoute;
        }
    }
}
