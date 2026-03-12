using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteOrderQuery request, CancellationToken cancellationToken)
        {
            //if (!await unitOfWork.Repository<Entities.Models.Territory>().GetExistsAsync(y => y.OrderId == request.Id && y.IsActive))
            //{
            var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            order.IsDelete = true;
            order.IsActive = false;
            order.ModifiedDate = DateTime.Now;
            order.DeleteDate = DateTime.Now;
            order.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Order>().Update(order);
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
