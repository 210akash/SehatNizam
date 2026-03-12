using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRouteHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRouteQuery request, CancellationToken cancellationToken)
        {
            //if (!await unitOfWork.Repository<RouteShop>().GetExistsAsync(y => y.RouteId == request.Id && y.IsActive) && !await unitOfWork.Repository<DSFRoute>().GetExistsAsync(y => y.RouteId == request.Id && y.IsActive))
            //{
                var route = await unitOfWork.Repository<Entities.Models.Route>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                route.IsDelete = true;
                route.IsActive = false;
                route.ModifiedDate = DateTime.Now;
                route.DeleteDate = DateTime.Now;
                route.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.Route>().Update(route);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            //}
            //else
            //    return (long)ResponseStatus.Conflict;
        }
    }
}
