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
    public class DeleteRouteShopHandler : IRequestHandler<DeleteRouteShopQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRouteShopHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRouteShopQuery request, CancellationToken cancellationToken)
        {
            var routeShop = await unitOfWork.Repository<Entities.Models.RouteShop>().GetFirstAsNoTrackingAsync(y => y.Id == request.RouteShopId);
            routeShop.IsDelete = true;
            routeShop.IsActive = false;
            routeShop.ModifiedDate = DateTime.Now;
            routeShop.DeleteDate = DateTime.Now;
            routeShop.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RouteShop>().Update(routeShop);
            var check = await unitOfWork.SaveChangesAsync();
            if (check > 0)
            {
                return (long)ResponseStatus.OK;
            }
            else
            {
                return (long)ResponseStatus.Error;
            }
        }
    }
}
