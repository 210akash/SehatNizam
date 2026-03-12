using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class ConfirmRetailOrderQuantityHandler : IRequestHandler<ConfirmRetailOrderQuantityCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ConfirmRetailOrderQuantityHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<ConfirmRetailOrderQuantityCommand, long>.Handle(ConfirmRetailOrderQuantityCommand request, CancellationToken cancellationToken)
        {
            var RetailOrder = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (RetailOrder != null)
            {
                foreach (var item in request.RetailOrderItemsList)
                {
                    var RetailOrderItem = await unitOfWork.Repository<RetailOrderItems>().GetFirstAsNoTrackingAsync(x => x.Id == item.Id);
                    RetailOrderItem.ShippedQuantity = item.ShippedQuantity;
                    RetailOrderItem.CustomTradePrice = item.CustomTradePrice;
                    RetailOrderItem.ModifiedDate = DateTime.Now;
                    RetailOrderItem.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<RetailOrderItems>().Update(RetailOrderItem);
                }

                var orderToUpdate = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
                orderToUpdate.ModifiedDate = DateTime.Now;
                orderToUpdate.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.RetailOrder>().Update(orderToUpdate);

                SaveChanges();
                return 200;

            }
            else
            {
                return 404;
            }
        }


    }
}