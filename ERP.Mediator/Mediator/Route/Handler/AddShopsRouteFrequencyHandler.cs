using System;
using System.Reflection;
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
    public class AddShopsRouteFrequencyHandler : IRequestHandler<AddShopsRouteFrequencyCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public AddShopsRouteFrequencyHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<AddShopsRouteFrequencyCommand, long>.Handle(AddShopsRouteFrequencyCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.RouteFrequencyList)
            {
                var routeShopFrequency = await unitOfWork.Repository<ShopRouteFrequency>().GetFirstAsNoTrackingAsync(x => x.ShopId == item.ShopId && x.IsActive == true);

                if (routeShopFrequency != null)
                {
                    routeShopFrequency.DeleteDate = DateTime.Now;
                    routeShopFrequency.ModifiedDate = DateTime.Now;
                    routeShopFrequency.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    routeShopFrequency.IsDelete = true;
                    routeShopFrequency.IsActive = false;
                    unitOfWork.Repository<ShopRouteFrequency>().Update(routeShopFrequency);
                    SaveChanges();

                }

                if (HasAtLeastOneTrueProperty(item.Schedule))
                {
                    var _routeShopFrequency = new ShopRouteFrequency();
                    _routeShopFrequency.ShopId = item.ShopId;
                    _routeShopFrequency.Monday = item.Schedule.Monday;
                    _routeShopFrequency.Tuesday = item.Schedule.Tuesday;
                    _routeShopFrequency.Wednesday = item.Schedule.Wednesday;
                    _routeShopFrequency.Thursday = item.Schedule.Thursday;
                    _routeShopFrequency.Friday = item.Schedule.Friday;
                    _routeShopFrequency.Saturday = item.Schedule.Saturday;
                    _routeShopFrequency.Sunday = item.Schedule.Sunday;
                    _routeShopFrequency.RouteId = request.RouteId;
                    _routeShopFrequency.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _routeShopFrequency.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<ShopRouteFrequency>().Add(_routeShopFrequency);
                    SaveChanges();
                }
            }
            SaveChanges();
            return 200;
        }

        static bool HasAtLeastOneTrueProperty(DaysOfWeek days)
        {
            // Get all properties of the DaysOfWeek object
            PropertyInfo[] properties = days.GetType().GetProperties();

            // Check if any boolean property is true
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(bool) && (bool)property.GetValue(days))
                {
                    return true; // Found a true property
                }
            }

            return false; // No true properties found
        }


    }
}