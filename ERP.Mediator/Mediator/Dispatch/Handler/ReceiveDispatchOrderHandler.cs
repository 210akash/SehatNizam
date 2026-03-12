using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class ReceiveDispatchOrderHandler : IRequestHandler<ReceiveDispatchOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ReceiveDispatchOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ReceiveDispatchOrderQuery request, CancellationToken cancellationToken)
        {
            //int check = 0;
            //var DispatchOrder = await unitOfWork.Repository<DispatchOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.DispatchOrderId);

            //if (DispatchOrder != null)
            //{
            //    DispatchOrder.StatusId = (long)OrderStatusEnum.OrderReceived;
            //    DispatchOrder.ReceivedById = sessionProvider.Session.LoggedInUserId;
            //    DispatchOrder.ReceivedDate = DateTime.Now;
            //    unitOfWork.Repository<DispatchOrder>().Update(DispatchOrder);

            //    check = await unitOfWork.SaveChangesAsync();
            //}

            //if (check > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            var _DispatchOrder = await unitOfWork.Repository<DispatchOrder>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == request.DispatchOrderId);

            if (_DispatchOrder != null)
            {
                _DispatchOrder.StatusId = (long)OrderStatusEnum.OrderReceived;
                _DispatchOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _DispatchOrder.ModifiedDate = DateTime.Now;
                _DispatchOrder.ReceivedById = sessionProvider.Session.LoggedInUserId;
                _DispatchOrder.ReceivedDate = DateTime.Now;
                unitOfWork.Repository<DispatchOrder>().Update(_DispatchOrder);

                OrderProcess process = new()
                {
                    FromStatusId = (long)OrderStatusEnum.OrderDispatched,
                    ToStatusId = (long)OrderStatusEnum.OrderReceived,
                    Comments = "Dispatch " + request.DispatchOrderId + " Received.",
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };
                unitOfWork.Repository<OrderProcess>().Add(process);
                var check = await unitOfWork.SaveChangesAsync();
                Expression<Func<Order, bool>> predicate = x => x.IsActive == true && x.Id == _DispatchOrder.OrderId;

                Expression<Func<Order, object>>[] includes = {
                            x => x.OrderItems,
                            x => x.DispatchOrder,
                        };

                List<string> thenInclude = new()
                {
                    "DispatchOrder.DispatchDetail"
                };

                var lObjOrderEntity = unitOfWork.Repository<Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);
                var _OrderEntity = lObjOrderEntity.Item1.ToList().FirstOrDefault();
                var TotalOrderItemQty = _OrderEntity.OrderItems.Where(x => x.IsActive == true).Sum(x => x.Quantity);
                var DispatchSumItem = _OrderEntity.DispatchOrder.Where(d => d.IsActive == true && d.StatusId == (long)OrderStatusEnum.OrderReceived).SelectMany(d => d.DispatchDetail).Where(dd => dd.IsActive == true).Sum(dd => dd.Quantity);

                if (TotalOrderItemQty == DispatchSumItem)
                {
                    var _UpdateOrder = await unitOfWork.Repository<Order>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == _OrderEntity.Id, null, null, null);

                    _UpdateOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _UpdateOrder.ModifiedDate = DateTime.Now;
                    _UpdateOrder.OrderStatusId = (long)OrderStatusEnum.OrderReceived;
                    unitOfWork.Repository<Order>().Update(_UpdateOrder);

                    OrderProcess lOrderRecvprocess = new OrderProcess();
                    lOrderRecvprocess.FromStatusId = (long)OrderStatusEnum.OrderDispatched;
                    lOrderRecvprocess.ToStatusId = (long)OrderStatusEnum.OrderReceived;
                    lOrderRecvprocess.Comments = "Order Completly Received";
                    lOrderRecvprocess.CreatedById = sessionProvider.Session.LoggedInUserId;
                    lOrderRecvprocess.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<OrderProcess>().Add(lOrderRecvprocess);
                    var OrderSaveCheck = await unitOfWork.SaveChangesAsync();
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
            else
            {
                return false;
            }
        }
    }
}
