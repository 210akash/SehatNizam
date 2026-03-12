using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dealership.Command;
using ERP.Mediator.Mediator.User.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class SaveDealershipHandler : IRequestHandler<SaveDealershipCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveDealershipHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveDealershipCommand, long>.Handle(SaveDealershipCommand request, CancellationToken cancellationToken)
        {
            if (request.DealershipTypeId == 1 && request.IsActive == true)
            {
                var checkDealershipTerritory = await unitOfWork.Repository<Entities.Models.Dealership>().GetExistsAsync(x => x.TerritoryId == request.TerritoryId && x.Id != request.Id && x.DealershipTypeId == 1 && x.IsActive == true && x.IsDelete == false);
                if (checkDealershipTerritory)
                {
                    var checkDealerships = await unitOfWork.Repository<Entities.Models.Dealership>().GetAsync(x => x.TerritoryId == request.TerritoryId && x.Id != request.Id && x.DealershipTypeId == 1 && x.IsActive == true && x.IsDelete == false);
                    return 409;
                }
            }

            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (dealership == null)
            {
                var _dealership = mapper.Map<Entities.Models.Dealership>(request);
                _dealership.CreatedById = sessionProvider.Session.LoggedInUserId;
                _dealership.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Dealership>().Add(_dealership);
                SaveChanges();

                if (request.DealershipImages != null)
                {
                    foreach (var item in request.DealershipImages)
                    {
                        Attachments attachment = new Attachments();
                        attachment.CreatedDate = DateTime.Now;
                        attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                        attachment.DealershipId = _dealership.Id;

                        BlobImageUploadModel blobModel = new()
                        {
                            File = item.FileSource,
                            FileName = item.ImageName,
                            FolderName = "assets/Files"
                        };

                        attachment.ImageName = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, item.Extension);
                        await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                    }
                }

                SaveChanges();
            }
            else
            {
                if (request.DealershipImages != null)
                {
                    var previousImages = await unitOfWork.Repository<Attachments>().GetAsync(x => x.DealershipId == request.Id && x.IsActive == true);

                    var previousImageIds = previousImages.Select(x => x.Id).ToList();
                    var currentImageIds = request.DealershipImages.Select(o => o.Id).ToList();
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

                    foreach (var item in request.DealershipImages)
                    {
                        if (item.Id == 0)
                        {
                            Attachments attachment = new Attachments();
                            attachment.CreatedDate = DateTime.Now;
                            attachment.CreatedById = this.sessionProvider.Session.LoggedInUserId;
                            attachment.DealershipId = request.Id;
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
                }

                var _dealership = mapper.Map<Entities.Models.Dealership>(request);
                _dealership.CreatedById = dealership.CreatedById;
                _dealership.CreatedDate = dealership.CreatedDate;
                _dealership.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _dealership.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Dealership>().Update(_dealership);

                var user = await this.unitOfWork.Repository<AspNetUsers>().GetFirstAsync(u => u.DealershipId == _dealership.Id);
                if (user != null)
                {
                    if (_dealership.IsActive == false)
                    {
                        // Disable user if dealership is now inactive
                        user.IsActive = false;
                        this.unitOfWork.Repository<AspNetUsers>().Update(user);
                    }
                    else if (_dealership.IsActive && dealership.IsActive == false)
                    {
                        // Enable user if dealership was inactive and is now active
                        user.IsActive = true;
                        this.unitOfWork.Repository<AspNetUsers>().Update(user);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}