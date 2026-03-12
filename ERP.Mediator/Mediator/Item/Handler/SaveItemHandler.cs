using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Item.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class SaveItemHandler : IRequestHandler<SaveItemCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveItemHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        async Task<long> IRequestHandler<SaveItemCommand, long>.Handle(SaveItemCommand request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(x => x.Id == request.ItemTypeId);

            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Item>().GetExistsAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id && x.CompanyId == sessionProvider.Session.CompanyId);
            var Exists = await unitOfWork.Repository<Entities.Models.Item>().GetExistsAsync(x => x.Id == request.Id);

            string _ItemCode = "";
            if (await unitOfWork.Repository<Entities.Models.Item>().GetExistsAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.ItemTypeId == request.ItemTypeId))
            {
                Func<IQueryable<Entities.Models.Item>, IOrderedQueryable<Entities.Models.Item>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ItemCode = await unitOfWork.Repository<Entities.Models.Item>().GetFirstAsNoTrackingAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.ItemTypeId == request.ItemTypeId, OrderByDesc, null);
                int No = Convert.ToInt32(new string(ItemCode.Code.TakeLast(4).ToArray())) + 1;
                _ItemCode = No.ToString().PadLeft(4, '0');
            }
            else
                _ItemCode = "0001";
            request.Code = ItemType.Code + _ItemCode;

            if (checkDuplicate == false)
            {
                if (Exists == false)
                {
                    var _Item = mapper.Map<Entities.Models.Item>(request);
                    _Item.CompanyId = sessionProvider.Session.CompanyId;
                    _Item.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _Item.CreatedDate = DateTime.Now;

                    if (request.ProductImage != null && request.ProductImage.FileSource != null)
                    {
                        BlobImageUploadModel blobModel = new()
                        {
                            File = request.ProductImage.FileSource,
                            FileName = request.ProductImage.ImageName,
                            FolderName = "assets/Files"
                        };
                        _Item.Image = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.ProductImage.Extension);
                    }
                    

                    unitOfWork.Repository<Entities.Models.Item>().Add(_Item);
                    SaveChanges();
                }
                else
                {
                    var masterupdate = request;
                    var detailupdate = masterupdate.ItemGroup;
                    masterupdate.ItemGroup = null;

                    var Item = await unitOfWork.Repository<Entities.Models.Item>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
                    var _Item = mapper.Map<Entities.Models.Item>(request);
                    _Item.CreatedById = Item.CreatedById;
                    _Item.CompanyId = Item.CompanyId;
                    _Item.CreatedDate = Item.CreatedDate;
                    _Item.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _Item.ModifiedDate = DateTime.Now;

                    if (request.ProductImage != null && request.ProductImage.FileSource != null)
                    {
                        BlobImageUploadModel blobModel = new()
                        {
                            File = request.ProductImage.FileSource,
                            FileName = request.ProductImage.ImageName,
                            FolderName = "assets/Files"
                        };
                        _Item.Image = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.ProductImage.Extension);
                    }

                    unitOfWork.Repository<Entities.Models.Item>().Update(_Item);
                   
                    var ItemGroupList = await unitOfWork.Repository<ItemGroup>()
                  .GetPagingWhereAsNoTrackingAsync(y => y.ItemId == request.Id && y.IsActive == true,
                  null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousCategoryStoreIds = ItemGroupList.Select(y => y.Id).ToList();
                    List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                    List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                    // Handle deletions
                    foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                    {
                        ItemGroup _ItemGroup = ItemGroupList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                        if (_ItemGroup != null)
                        {
                            _ItemGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _ItemGroup.DeleteDate = DateTime.Now;
                            _ItemGroup.IsActive = false; // Soft delete
                            _ItemGroup.IsDelete = true; // Soft delete
                            unitOfWork.Repository<ItemGroup>().Update(_ItemGroup);
                        }
                    }

                    // Handle additions
                    foreach (var ItemGroup in detailupdate)
                    {
                        if (ItemGroup.Id != 0)
                        {
                            var updatedetail = await unitOfWork.Repository<ItemGroup>().GetFirstAsync(x => x.Id == ItemGroup.Id);
                            updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            updatedetail.ModifiedDate = DateTime.Now;
                            updatedetail.Name = ItemGroup.Name;
                            updatedetail.Description = ItemGroup.Description;
                            unitOfWork.Repository<ItemGroup>().Update(updatedetail);
                        }
                        else
                        {
                            var _IssuanceDetail = mapper.Map<ItemGroup>(ItemGroup);
                            _IssuanceDetail.ItemId = request.Id;
                            _IssuanceDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                            _IssuanceDetail.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<ItemGroup>().Add(_IssuanceDetail);
                        }
                    }

                    SaveChanges();
                }
                return 200;
            }
            else
            {
                return 409;
            }
        }
    }
}