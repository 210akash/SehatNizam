using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PrimaryOrder.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class SaveOrderHandler : IRequestHandler<CreateOrderCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveOrderHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<CreateOrderCommand, long>.Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var order = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (order == null)
            {
                var _order = mapper.Map<Entities.Models.Order>(request);
                _order.CreatedById = sessionProvider.Session.LoggedInUserId;
                _order.CreatedDate = request.CreatedDate == null ? DateTime.Now : request.CreatedDate;
                _order.OrderStatusId = (long)OrderStatusEnum.OrderCreate;
                _order.OrderAttachments = null;
                unitOfWork.Repository<Entities.Models.Order>().Add(_order);
                SaveChanges();

                foreach (var item in request.OrderItemsList)
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
                process.Comments = "New Order Created";
                process.CreatedById = sessionProvider.Session.LoggedInUserId;
                process.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

                foreach (var item in request.OrderAttachments)
                {
                    Attachments attachment = new Attachments();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                    attachment.OrderId = _order.Id;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = item.FileSource,
                        FileName = item.ImageName,
                        FolderName = "assets/Files"
                    };

                    attachment.ImageName = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, item.Extension);
                    await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                }

                SaveChanges();
            }
            else
            {
                var _order = mapper.Map<Entities.Models.Order>(request);
                _order.CreatedById = order.CreatedById;
                _order.CreatedDate = request.CreatedDate == null ? DateTime.Now : request.CreatedDate;
                _order.OrderStatusId = order.OrderStatusId;
                _order.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _order.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Order>().Update(_order);

                foreach (var item in request.OrderItemsList)
                {
                    var orderItemExisting = await unitOfWork.Repository<Entities.Models.OrderItems>().GetFirstAsNoTrackingAsync(x => x.Id == item.Id);
                    var _orderItems = mapper.Map<Entities.Models.OrderItems>(item);
                    _orderItems.OrderId = _order.Id;
                    _orderItems.IsActive = orderItemExisting.IsActive;
                    _orderItems.IsDelete = orderItemExisting.IsDelete;
                    _orderItems.CreatedById = orderItemExisting.CreatedById;
                    _orderItems.CreatedDate = orderItemExisting.CreatedDate;
                    _orderItems.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _orderItems.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.OrderItems>().Update(_orderItems);
                }

                var previousImages = await unitOfWork.Repository<Attachments>().GetAsync(x => x.OrderId == request.Id && x.IsActive == true);

                var previousImageIds = previousImages.Select(x => x.Id).ToList();
                var currentImageIds = request.OrderAttachments.Select(o => o.Id).ToList();
                var deletedImageIds = previousImageIds.Except(currentImageIds).ToList();

                foreach (var deletedImageId in deletedImageIds)
                {
                    var imageToDelete = previousImages.FirstOrDefault(x => x.Id == deletedImageId);
                    if (imageToDelete != null)
                    {
                        imageToDelete.IsDelete = true;
                        imageToDelete.IsActive = false;
                        imageToDelete.ModifiedDate = DateTime.Now;
                        imageToDelete.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        unitOfWork.Repository<Attachments>().Update(imageToDelete);
                        SaveChanges();
                    }
                }

                foreach (var item in request.OrderAttachments)
                {
                    if (item.Id == 0)
                    {
                        Attachments attachment = new Attachments();
                        attachment.CreatedDate = DateTime.Now;
                        attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                        attachment.OrderId = request.Id;
                        BlobImageUploadModel blobModel = new()
                        {
                            File = item.FileSource,
                            FileName = item.ImageName,
                            FolderName = "assets/Files"
                        };

                        attachment.ImageName = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel);
                        unitOfWork.Repository<Attachments>().Add(attachment);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}