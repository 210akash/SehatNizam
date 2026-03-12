using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class DeleteShopOrderHandler : IRequestHandler<DeleteShopOrderQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteShopOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteShopOrderQuery request, CancellationToken cancellationToken)
        {
            //if (!await unitOfWork.Repository<Entities.Models.Territory>().GetExistsAsync(y => y.ShopOrderId == request.Id && y.IsActive))
            //{
            var shopOrder = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            shopOrder.IsDelete = true;
            shopOrder.IsActive = false;
            shopOrder.ModifiedDate = DateTime.Now;
            shopOrder.DeleteDate = DateTime.Now;
            shopOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Order>().Update(shopOrder);
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
