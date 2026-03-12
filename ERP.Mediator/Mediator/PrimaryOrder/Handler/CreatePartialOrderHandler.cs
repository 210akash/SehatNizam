using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PrimaryOrder.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class CreatePartialOrderHandler : IRequestHandler<CreatePartialOrderCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public CreatePartialOrderHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<CreatePartialOrderCommand, long>.Handle(CreatePartialOrderCommand command, CancellationToken cancellationToken)
        {
            var _order = new Entities.Models.Order
            {
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                OrderStatusId = (long)OrderStatusEnum.OrderConfirm,
                DealershipId = command.DealershipId,
                DealershipAddress = command.DealershipAddress,
                IsPartial = true
            };

            unitOfWork.Repository<Entities.Models.Order>().Add(_order);
            SaveChanges();

            foreach (var item in command.PartialOrderItemsCommand)
            {
                if (item.Quantity > 0)
                {
                    var _orderItems = mapper.Map<Entities.Models.OrderItems>(item);
                    if (sessionProvider.Session.LoggedInUserId.ToString().ToUpper() == "E3D15615-46FB-4EE3-8FDD-44CDF68E930E")
                    {
                        _orderItems.CustomTradePrice = _orderItems.TradePrice;
                    }
                    _orderItems.IsActive = true;
                    _orderItems.IsDelete = false;
                    _orderItems.OrderId = _order.Id;
                    _orderItems.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _orderItems.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderItems>().Add(_orderItems);
                }
            }

            OrderProcess process = new OrderProcess();
            process.OrderId = _order.Id;
            process.FromStatusId = null;
            process.ToStatusId = _order.OrderStatusId;
            process.Comments = "New Partial Order Created against Order Id " + _order.Id;
            process.CreatedById = sessionProvider.Session.LoggedInUserId;
            process.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

            Attachments attachment = new Attachments();
            attachment.OrderId = _order.Id;
            attachment.ImageName = command.ImageName;
            attachment.CreatedById = sessionProvider.Session.LoggedInUserId;
            attachment.CreatedDate = DateTime.Now;
            unitOfWork.Repository<Entities.Models.Attachments>().Add(attachment);

            SaveChanges();

            return 200;
        }
    }
}