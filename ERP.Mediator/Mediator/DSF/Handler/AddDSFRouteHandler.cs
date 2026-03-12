using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.DSF.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DSF.Handler
{
    public class AddDSFRouteHandler : IRequestHandler<AddDSFRouteCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public AddDSFRouteHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        async Task<long> IRequestHandler<AddDSFRouteCommand, long>.Handle(AddDSFRouteCommand request, CancellationToken cancellationToken)
        {
            if (request.DSF.DSFRoute.Count == 0)
            {
                foreach (var item in request.RoutesToAdd)
                {
                    var _routeDSF = new DSFRoute();
                    _routeDSF.RouteId = item.Id;
                    _routeDSF.DSFId = request.DSF.Id;
                    _routeDSF.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _routeDSF.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<DSFRoute>().Add(_routeDSF);
                }
                unitOfWork.SaveChanges();
            }
            else
            {
                var previousRoutes = await unitOfWork.Repository<DSFRoute>().GetAsync(x => x.DSFId == request.DSF.Id && x.IsActive == true);

                var previousRouteIds = previousRoutes.Select(x => x.RouteId).ToList();
                var currentRouteIds = request.RoutesToAdd.Select(o => o.Id).ToList();
                var deletedRouteIds = previousRouteIds.Except(currentRouteIds).ToList();

                foreach (var deletedRouteId in deletedRouteIds)
                {
                    var routesToDelete = previousRoutes.FirstOrDefault(x => x.RouteId == deletedRouteId);
                    if (routesToDelete != null)
                    {
                        routesToDelete.IsDelete = true;
                        routesToDelete.IsActive = false;
                        routesToDelete.ModifiedDate = DateTime.Now;
                        routesToDelete.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        unitOfWork.Repository<DSFRoute>().Update(routesToDelete);
                        unitOfWork.SaveChanges();
                    }
                }

                foreach (var routeToAdd in request.RoutesToAdd)
                {
                    var routeToAddCheck = await unitOfWork.Repository<DSFRoute>().GetFirstAsNoTrackingAsync(x => x.RouteId == routeToAdd.Id && x.IsActive == true);
                    if (routeToAddCheck == null)
                    {
                        var newDSFRoute = new DSFRoute
                        {
                            RouteId = routeToAdd.Id,
                            DSFId = request.DSF.Id,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now
                        };
                        unitOfWork.Repository<DSFRoute>().Add(newDSFRoute);
                    }
                }

                unitOfWork.SaveChanges();


            }

            return 200;
        }
    }
}
