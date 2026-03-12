using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Account.Command;
using ERP.Mediator.Mediator.AuditReview.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AuditReview.Handler
{
    public class SaveAuditReviewHandler : IRequestHandler<SaveAuditReviewCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAuditReviewHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAuditReviewCommand, long>.Handle(SaveAuditReviewCommand request, CancellationToken cancellationToken)
        {
            var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.Id == request.OrderId);
            if (order != null)
            {
                if (order.OrderStatusId == (long)OrderStatusEnum.OrderInProcess)
                {
                    if (request.IsTransactionLedgerEntry)
                    {
                        //Handle LEdger Impact
                    }
                    var _order = mapper.Map<Entities.Models.Order>(order);
                    _order.CreatedById = order.CreatedById;
                    _order.CreatedDate = order.CreatedDate;
                    _order.OrderStatusId = (long)OrderStatusEnum.AccountReviewed;
                    _order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _order.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Order>().Update(_order);

                    OrderProcess process = new OrderProcess();
                    process.OrderId = _order.Id;
                    process.FromStatusId = (long)OrderStatusEnum.OrderInProcess;
                    process.ToStatusId = (long)OrderStatusEnum.AccountReviewed;
                    process.Comments = request.Description;
                    process.CreatedById = sessionProvider.Session.LoggedInUserId;
                    process.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
                    SaveChanges();
                }
                else if (order.OrderStatusId == (long)OrderStatusEnum.AccountReviewed)
                {
                    if (request.IsTransactionLedgerEntry)
                    {
                        //Handle LEdger Impact
                    }
                    var _order = mapper.Map<Entities.Models.Order>(order);
                    _order.CreatedById = order.CreatedById;
                    _order.CreatedDate = order.CreatedDate;
                    _order.OrderStatusId = (long)OrderStatusEnum.ManagerApproved;
                    _order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _order.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Order>().Update(_order);

                    OrderProcess process = new OrderProcess();
                    process.OrderId = _order.Id;
                    process.FromStatusId = (long)OrderStatusEnum.AccountReviewed;
                    process.ToStatusId = (long)OrderStatusEnum.ManagerApproved;
                    process.Comments = request.Description;
                    process.CreatedById = sessionProvider.Session.LoggedInUserId;
                    process.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
                    SaveChanges();
                }
                else if (order.OrderStatusId == (long)OrderStatusEnum.ManagerApproved)
                {
                    if (request.IsTransactionLedgerEntry)
                    {
                        //Handle LEdger Impact
                    }
                    var _order = mapper.Map<Entities.Models.Order>(order);
                    _order.CreatedById = order.CreatedById;
                    _order.CreatedDate = order.CreatedDate;
                    _order.OrderStatusId = (long)OrderStatusEnum.OrderConfirm;
                    _order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _order.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Order>().Update(_order);

                    OrderProcess process = new OrderProcess();
                    process.OrderId = _order.Id;
                    process.FromStatusId = (long)OrderStatusEnum.ManagerApproved;
                    process.ToStatusId = (long)OrderStatusEnum.OrderConfirm;
                    process.Comments = request.Description;
                    process.CreatedById = sessionProvider.Session.LoggedInUserId;
                    process.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
                    SaveChanges();
                }
                return 200;
            }
            return 400;
        }

    }
}
