using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Route.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class AddShopsRouteHandler : IRequestHandler<AddShopsRouteCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public AddShopsRouteHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<AddShopsRouteCommand, long>.Handle(AddShopsRouteCommand request, CancellationToken cancellationToken)
        {
            if (request.Route.RouteShop.Count == 0)
            {
                foreach (var item in request.ShopsToAdd)
                {
                    var _routeShop = new Entities.Models.RouteShop();
                    _routeShop.SequenceNo = item.Sequence;
                    _routeShop.ShopId = item.Id;
                    _routeShop.RouteId = request.Route.Id;
                    _routeShop.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _routeShop.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.RouteShop>().Add(_routeShop);
                }
                SaveChanges();
            }
            else
            {
                //var previousRouteShops = await unitOfWork.Repository<RouteShop>().GetAsync(x => x.RouteId == request.Route.Id && x.IsActive == true);

                //var previousRouteShopIds = previousRouteShops.Select(x => x.ShopId).ToList();
                //var currentRouteShopIds = request.ShopsToAdd.Select(o => o.Id).ToList();
                //var deletedRouteShopIds = previousRouteShopIds.Except(currentRouteShopIds).ToList();

                //foreach (var deletedRouteShopId in deletedRouteShopIds)
                //{
                //    var routeShopToDelete = previousRouteShops.FirstOrDefault(x => x.ShopId == deletedRouteShopId);
                //    if (routeShopToDelete != null)
                //    {
                //        routeShopToDelete.IsDelete = true;
                //        routeShopToDelete.IsActive = false;
                //        routeShopToDelete.ModifiedDate = DateTime.Now;
                //        routeShopToDelete.ModifiedById = sessionProvider.Session.LoggedInUserId;
                //        unitOfWork.Repository<RouteShop>().Update(routeShopToDelete);
                //        SaveChanges();
                //    }
                //}

                var previousRouteShops = await unitOfWork.Repository<RouteShop>().GetAsync(x => x.RouteId == request.Route.Id && x.IsActive == true);
                
                foreach (var previousRouteShop in previousRouteShops)
                {
                    previousRouteShop.IsDelete = true;
                    previousRouteShop.IsActive = false;
                    previousRouteShop.ModifiedDate = DateTime.Now;
                    previousRouteShop.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<RouteShop>().Update(previousRouteShop);
                    SaveChanges();
                }
                
                foreach (var shopToAdd in request.ShopsToAdd)
                {
                    var routeShopToAddCheck = await unitOfWork.Repository<RouteShop>().GetFirstAsNoTrackingAsync(x => x.ShopId == shopToAdd.Id && x.IsActive == true);
                    if (routeShopToAddCheck == null)
                    {
                        var newRouteShop = new RouteShop
                        {
                            SequenceNo = shopToAdd.Sequence,
                            ShopId = shopToAdd.Id,
                            RouteId = request.Route.Id,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now
                        };
                        unitOfWork.Repository<RouteShop>().Add(newRouteShop);
                    }
                }

                SaveChanges();


            }
            return 200;
        }


    }
}