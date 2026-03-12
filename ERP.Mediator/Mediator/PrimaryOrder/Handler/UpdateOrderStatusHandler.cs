using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public UpdateOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(UpdateOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == request.OrderId);
            order.OrderStatusId = request.ToStatusId;
            order.ModifiedDate = DateTime.Now;
            order.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Order>().Update(order);

            OrderProcess process = new OrderProcess();
            process.OrderId = request.OrderId;
            process.FromStatusId = request.FromStatusId;
            process.ToStatusId = request.ToStatusId;
            process.Comments = request.Comments;
            process.TransactionId = request.TransactionId;
            process.CreatedById = sessionProvider.Session.LoggedInUserId;
            process.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

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
