using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.WarehouseTransfer.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Handler
{
    public class SaveWarehouseTransferHandler : IRequestHandler<SaveWarehouseTransferCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveWarehouseTransferHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveWarehouseTransferCommand, long>.Handle(SaveWarehouseTransferCommand request, CancellationToken cancellationToken)
        {
            var WarehouseTransfer = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (WarehouseTransfer == null)
            {
                string _WarehouseTransferCode = "";
                if (await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId))
                {
                    Func<IQueryable<Entities.Models.WarehouseTransfer>, IOrderedQueryable<Entities.Models.WarehouseTransfer>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var WarehouseTransferCode = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                    int No = Convert.ToInt32(WarehouseTransferCode.Code) + 1;
                    _WarehouseTransferCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _WarehouseTransferCode = "0000001";
                request.Code = _WarehouseTransferCode;

                var _WarehouseTransfer = mapper.Map<Entities.Models.WarehouseTransfer>(request);
                _WarehouseTransfer.CompanyId = sessionProvider.Session.CompanyId;
                _WarehouseTransfer.CreatedById = sessionProvider.Session.LoggedInUserId;
                _WarehouseTransfer.TransferFromId = sessionProvider.Session.SelectedWarehouseId;
                _WarehouseTransfer.CreatedDate = DateTime.Now;
                _WarehouseTransfer.StatusId = 1;

                _WarehouseTransfer.WarehouseTransferDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.WarehouseTransfer>().Add(_WarehouseTransfer);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.WarehouseTransferDetail;
                masterupdate.WarehouseTransferDetail = null;
                var _WarehouseTransfer = mapper.Map<Entities.Models.WarehouseTransfer>(masterupdate);
                _WarehouseTransfer.StatusId = WarehouseTransfer.StatusId;
                _WarehouseTransfer.CreatedById = WarehouseTransfer.CreatedById;
                _WarehouseTransfer.CompanyId = WarehouseTransfer.CompanyId;
                _WarehouseTransfer.CreatedDate = WarehouseTransfer.CreatedDate;
                _WarehouseTransfer.TransferFromId = WarehouseTransfer.TransferFromId;
                _WarehouseTransfer.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _WarehouseTransfer.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.WarehouseTransfer>().Update(_WarehouseTransfer);

                var CategoryStoreList = await unitOfWork.Repository<WarehouseTransferDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.WarehouseTransferId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    WarehouseTransferDetail _WarehouseTransferDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_WarehouseTransferDetail != null)
                    {
                        _WarehouseTransferDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _WarehouseTransferDetail.DeleteDate = DateTime.Now;
                        _WarehouseTransferDetail.IsActive = false; // Soft delete
                        _WarehouseTransferDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<WarehouseTransferDetail>().Update(_WarehouseTransferDetail);
                    }
                }

                // Handle additions
                foreach (var WarehouseTransferD in detailupdate)
                {
                    if (WarehouseTransferD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<WarehouseTransferDetail>()
                                .GetFirstAsync(x => x.Id == WarehouseTransferD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.ItemId = WarehouseTransferD.ItemId;
                        updatedetail.Quantity = WarehouseTransferD.Quantity;
                        unitOfWork.Repository<WarehouseTransferDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _WarehouseTransferDetail = mapper.Map<WarehouseTransferDetail>(WarehouseTransferD);
                        _WarehouseTransferDetail.WarehouseTransferId = request.Id;
                        _WarehouseTransferDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _WarehouseTransferDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<WarehouseTransferDetail>().Add(_WarehouseTransferDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}