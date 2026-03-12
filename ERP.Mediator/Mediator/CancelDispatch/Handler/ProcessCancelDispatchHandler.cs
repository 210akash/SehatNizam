using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.CancelDispatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CancelDispatch.Handler
{
    public class ProcessCancelDispatchHandler : IRequestHandler<ProcessCancelDispatchCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessCancelDispatchHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessCancelDispatchCommand request, CancellationToken cancellationToken)
        {
            int check = 0;
            var cancelDispatch = await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            if (cancelDispatch != null)
            {
                if (request.StatusId == (long)OrderStatusEnum.CancelDispatchConfirm)
                {
                    bool checkDispatched = await AreAllDispatchReceived(cancelDispatch.OrderId);

                    if (checkDispatched == true)
                    {
                        var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == cancelDispatch.OrderId);
                        if (order != null)
                        {
                            order.OrderStatusId = (long)OrderStatusEnum.OrderReceived;
                            order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            order.ModifiedDate = DateTime.Now;
                            unitOfWork.Repository<Entities.Models.Order>().Update(order);

                            OrderProcess orderProcess = new OrderProcess();
                            orderProcess.OrderId = cancelDispatch.OrderId;
                            orderProcess.FromStatusId = order.OrderStatusId;
                            orderProcess.ToStatusId = (long)OrderStatusEnum.OrderReceived;
                            orderProcess.Comments = request.Remarks;
                            orderProcess.CreatedById = sessionProvider.Session.LoggedInUserId;
                            orderProcess.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(orderProcess);
                        }
                    }
                }

                OrderProcess process = new OrderProcess();
                process.CancelDispatchId = request.Id;
                process.FromStatusId = cancelDispatch.StatusId;
                process.ToStatusId = request.StatusId;
                process.IsReject = request.IsReject;
                process.Comments = request.Remarks;
                process.CreatedById = sessionProvider.Session.LoggedInUserId;
                process.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

                cancelDispatch.StatusId = request.StatusId;
                cancelDispatch.ModifiedDate = DateTime.Now;
                cancelDispatch.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.CancelDispatch>().Update(cancelDispatch);

                check = await unitOfWork.SaveChangesAsync();
            }

            if (check > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AreAllDispatchReceived(long orderId)
        {
            var dispatchOrders = await unitOfWork.Repository<DispatchOrder>()
                .GetAsync(x => x.OrderId == orderId);

            if (!dispatchOrders.Any())
                return false;

            bool allApproved = dispatchOrders.All(d => d.StatusId == (long)OrderStatusEnum.OrderReceived);

            return allApproved;
        }


    }
}
