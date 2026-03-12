using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Shop.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class SaveShopHandler : IRequestHandler<SaveShopCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveShopHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveShopCommand, long>.Handle(SaveShopCommand request, CancellationToken cancellationToken)
        {
            var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (shop == null)
            {
                //New Shop
                var duplicationShop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.PhoneNo == request.PhoneNo && x.IsActive == true);
                if (duplicationShop != null)
                {
                    return (long)ResponseStatus.DuplicatePhoneNo;
                }

                var _shop = mapper.Map<Entities.Models.Shop>(request);
                _shop.CreatedById = sessionProvider.Session.LoggedInUserId;
                _shop.CreatedDate = DateTime.Now;
                _shop.IsVerified = true;
                _shop.IsTagFromMob = false;
                _shop.StatusId = 1;

                unitOfWork.Repository<Entities.Models.Shop>().Add(_shop);
                SaveChanges();

                foreach (var item in request.ShopImages)
                {
                    Attachments attachment = new Attachments();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                    attachment.ShopId = _shop.Id;

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
                var duplicationShop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(x => x.PhoneNo == request.PhoneNo && x.Id != request.Id && x.IsActive == true);
                if (duplicationShop != null)
                {
                    return (long)ResponseStatus.DuplicatePhoneNo;
                }
                var previousImages = await unitOfWork.Repository<Attachments>().GetAsync(x => x.ShopId == request.Id && x.IsActive == true);

                var previousImageIds = previousImages.Select(x => x.Id).ToList();
                var currentImageIds = request.ShopImages.Select(o => o.Id).ToList();
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

                foreach (var item in request.ShopImages)
                {
                    if (item.Id == 0)
                    {
                        Attachments attachment = new Attachments();
                        attachment.CreatedDate = DateTime.Now;
                        attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                        attachment.ShopId = request.Id;
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

                var _shop = mapper.Map<Entities.Models.Shop>(request);
                _shop.IsVerified = shop.IsVerified;
                _shop.VerifiedDate = shop.VerifiedDate;
                _shop.VerifiedById = shop.VerifiedById;
                _shop.CreatedById = shop.CreatedById;
                _shop.CreatedDate = shop.CreatedDate;
                _shop.IsTagFromMob = shop.IsTagFromMob;
                _shop.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _shop.ModifiedDate = DateTime.Now;
                _shop.StatusId = shop.StatusId;
                unitOfWork.Repository<Entities.Models.Shop>().Update(_shop);
                SaveChanges();
            }
            return 200;
        }
    }
}