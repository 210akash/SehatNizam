using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class UpdateShopOrderStatusHandler : IRequestHandler<UpdateShopOrderStatusQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public UpdateShopOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(UpdateShopOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var shopOrder = await unitOfWork.Repository<Entities.Models.ShopOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.ShopOrderId);
            shopOrder.ShopOrderStatusId = request.ToStatusId;
            shopOrder.ModifiedDate = DateTime.Now;
            shopOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ShopOrder>().Update(shopOrder);

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
