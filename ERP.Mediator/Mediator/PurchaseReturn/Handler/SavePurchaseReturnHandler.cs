using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseReturn.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class SavePurchaseReturnHandler : IRequestHandler<SavePurchaseReturnCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePurchaseReturnHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePurchaseReturnCommand, long>.Handle(SavePurchaseReturnCommand request, CancellationToken cancellationToken)
        {
            var PurchaseReturn = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (PurchaseReturn == null)
            {
                string _PurchaseReturnCode = "";
                if (await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetExistsAsync(y => y.IsActive))
                {
                    Func<IQueryable<Entities.Models.PurchaseReturn>, IOrderedQueryable<Entities.Models.PurchaseReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var PurchaseReturnCode = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(PurchaseReturnCode.Code) + 1;
                    _PurchaseReturnCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _PurchaseReturnCode = "0000001";
                request.Code = _PurchaseReturnCode;

                var _PurchaseReturn = mapper.Map<Entities.Models.PurchaseReturn>(request);
                _PurchaseReturn.CreatedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseReturn.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _PurchaseReturn.CreatedDate = DateTime.Now;
                _PurchaseReturn.StatusId = 1;

                _PurchaseReturn.PurchaseReturnDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.PurchaseReturn>().Add(_PurchaseReturn);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.PurchaseReturnDetail;
                masterupdate.PurchaseReturnDetail = null;
                var _PurchaseReturn = mapper.Map<Entities.Models.PurchaseReturn>(masterupdate);
                _PurchaseReturn.Code = PurchaseReturn.Code;
                _PurchaseReturn.StatusId = PurchaseReturn.StatusId;
                _PurchaseReturn.CreatedById = PurchaseReturn.CreatedById;
                _PurchaseReturn.CreatedDate = PurchaseReturn.CreatedDate;
                _PurchaseReturn.ProjectId = PurchaseReturn.ProjectId;
                _PurchaseReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseReturn.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.PurchaseReturn>().Update(_PurchaseReturn);


                var PurchaseReturnDetailsList = await unitOfWork.Repository<PurchaseReturnDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.PurchaseReturnId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousPurchaseReturnDetailIds = PurchaseReturnDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousPurchaseReturnDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    PurchaseReturnDetail _PurchaseReturnDetails = PurchaseReturnDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_PurchaseReturnDetails != null)
                    {
                        _PurchaseReturnDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseReturnDetails.DeleteDate = DateTime.Now;
                        _PurchaseReturnDetails.IsActive = false;
                        _PurchaseReturnDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.PurchaseReturnDetail>().Update(_PurchaseReturnDetails);
                    }
                }

                foreach (var PurchaseReturnD in detailupdate)
                {
                    if (PurchaseReturnD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<PurchaseReturnDetail>()
                           .GetFirstAsync(x => x.Id == PurchaseReturnD.Id);
                        updatedetail.GRNDetailId = PurchaseReturnD.GRNDetailId;
                        updatedetail.Quantity = PurchaseReturnD.Quantity;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<PurchaseReturnDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _PurchaseReturnDetails = mapper.Map<PurchaseReturnDetail>(PurchaseReturnD);
                        _PurchaseReturnDetails.PurchaseReturnId = request.Id;
                        _PurchaseReturnDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseReturnDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<PurchaseReturnDetail>().Add(_PurchaseReturnDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}