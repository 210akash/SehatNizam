using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class DeleteDispatchHandler : IRequestHandler<DeleteDispatchQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteDispatchHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteDispatchQuery request, CancellationToken cancellationToken)
        {
            var Dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null,null, "DispatchOrder,DispatchOrder.DispatchDetail");
            Dispatch.IsDelete = true;
            Dispatch.IsActive = false;
            Dispatch.DeleteDate = DateTime.Now;
            Dispatch.ModifiedDate = DateTime.Now;
            Dispatch.ModifiedById = sessionProvider.Session.LoggedInUserId;

            foreach (var DispatchOrder in Dispatch.DispatchOrder)
            {
                DispatchOrder.IsDelete = true;
                DispatchOrder.IsActive = false;
                DispatchOrder.DeleteDate = DateTime.Now;
                DispatchOrder.ModifiedDate = DateTime.Now;

                var Order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == DispatchOrder.OrderId);
                Order.OrderStatusId = 30;
                unitOfWork.Repository<Entities.Models.Order>().Update(Order);

                foreach (var DispatchDetail in DispatchOrder.DispatchDetail)
                {
                    DispatchDetail.IsDelete = true;
                    DispatchDetail.IsActive = false;
                    DispatchDetail.DeleteDate = DateTime.Now;
                    DispatchDetail.ModifiedDate = DateTime.Now;
                }
            }

            unitOfWork.Repository<Entities.Models.Dispatch>().Update(Dispatch);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
