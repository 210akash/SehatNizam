using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class SavePurchaseOrderHandler : IRequestHandler<SavePurchaseOrderCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePurchaseOrderHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePurchaseOrderCommand, long>.Handle(SavePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (PurchaseOrder == null)
            {
                string _PurchaseOrderCode = "";
                if (await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.PurchaseOrder>, IOrderedQueryable<Entities.Models.PurchaseOrder>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var PurchaseOrderCode = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetOneAsync(y => y.IsActive == true , OrderByDesc, null);
                    int No = Convert.ToInt32(PurchaseOrderCode.Code) + 1;
                    _PurchaseOrderCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _PurchaseOrderCode = "0000001";
                request.Code = _PurchaseOrderCode;

                var _PurchaseOrder = mapper.Map<Entities.Models.PurchaseOrder>(request);
                _PurchaseOrder.CreatedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseOrder.CompanyId = sessionProvider.Session.CompanyId;
                _PurchaseOrder.CreatedDate = DateTime.Now;
                _PurchaseOrder.StatusId = 1;

                _PurchaseOrder.PurchaseOrderDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.PurchaseOrder>().Add(_PurchaseOrder);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.PurchaseOrderDetail;
                masterupdate.PurchaseOrderDetail = null;
                var _PurchaseOrder = mapper.Map<Entities.Models.PurchaseOrder>(masterupdate);
                _PurchaseOrder.Code = PurchaseOrder.Code;
                _PurchaseOrder.StatusId = PurchaseOrder.StatusId;
                _PurchaseOrder.CreatedById = PurchaseOrder.CreatedById;
                _PurchaseOrder.CreatedDate = PurchaseOrder.CreatedDate;
                _PurchaseOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseOrder.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.PurchaseOrder>().Update(_PurchaseOrder);

                var CategoryStoreList = await unitOfWork.Repository<PurchaseOrderDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.PurchaseOrderId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    PurchaseOrderDetail _PurchaseOrderDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_PurchaseOrderDetail != null)
                    {
                        _PurchaseOrderDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseOrderDetail.DeleteDate = DateTime.Now;
                        _PurchaseOrderDetail.IsActive = false; // Soft delete
                        _PurchaseOrderDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.PurchaseOrderDetail>().Update(_PurchaseOrderDetail);
                    }
                }

                // Handle additions
                foreach (var PurchaseOrderD in detailupdate)
                {
                    if (PurchaseOrderD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<PurchaseOrderDetail>()
                                .GetFirstAsync(x => x.Id == PurchaseOrderD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.Quantity = PurchaseOrderD.Quantity;
                        updatedetail.Description = PurchaseOrderD.Description;
                        unitOfWork.Repository<PurchaseOrderDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _PurchaseOrderDetail = mapper.Map<PurchaseOrderDetail>(PurchaseOrderD);
                        _PurchaseOrderDetail.PurchaseOrderId = request.Id;
                        _PurchaseOrderDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseOrderDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.PurchaseOrderDetail>().Add(_PurchaseOrderDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}