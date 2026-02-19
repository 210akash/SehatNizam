using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseDemand.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class SavePurchaseDemandHandler : IRequestHandler<SavePurchaseDemandCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePurchaseDemandHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePurchaseDemandCommand, long>.Handle(SavePurchaseDemandCommand request, CancellationToken cancellationToken)
        {
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (PurchaseDemand == null)
            {
                string _PurchaseDemandCode = "";
                if (await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.PurchaseDemand>, IOrderedQueryable<Entities.Models.PurchaseDemand>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var PurchaseDemandCode = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetOneAsync(y => y.IsActive == true , OrderByDesc, null);
                    int No = Convert.ToInt32(PurchaseDemandCode.Code) + 1;
                    _PurchaseDemandCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _PurchaseDemandCode = "0000001";
                request.Code = _PurchaseDemandCode;

                var _PurchaseDemand = mapper.Map<Entities.Models.PurchaseDemand>(request);
                _PurchaseDemand.CreatedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseDemand.CreatedDate = DateTime.Now;
                _PurchaseDemand.StatusId = 1;
                _PurchaseDemand.StoreId = sessionProvider.Session.StoreId;

                _PurchaseDemand.PurchaseDemandDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.PurchaseDemand>().Add(_PurchaseDemand);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.PurchaseDemandDetail;
                masterupdate.PurchaseDemandDetail = null;
                var _PurchaseDemand = mapper.Map<Entities.Models.PurchaseDemand>(masterupdate);
                _PurchaseDemand.Code = PurchaseDemand.Code;
                _PurchaseDemand.StatusId = PurchaseDemand.StatusId;
                _PurchaseDemand.StoreId = PurchaseDemand.StoreId;
                _PurchaseDemand.CreatedById = PurchaseDemand.CreatedById;
                _PurchaseDemand.CreatedDate = PurchaseDemand.CreatedDate;
                _PurchaseDemand.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _PurchaseDemand.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.PurchaseDemand>().Update(_PurchaseDemand);

                var CategoryStoreList = await unitOfWork.Repository<PurchaseDemandDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.PurchaseDemandId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    PurchaseDemandDetail _PurchaseDemandDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_PurchaseDemandDetail != null)
                    {
                        _PurchaseDemandDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseDemandDetail.DeleteDate = DateTime.Now;
                        _PurchaseDemandDetail.IsActive = false; // Soft delete
                        _PurchaseDemandDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.PurchaseDemandDetail>().Update(_PurchaseDemandDetail);
                    }
                }

                // Handle additions
                foreach (var PurchaseDemandD in detailupdate)
                {
                    if (PurchaseDemandD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<PurchaseDemandDetail>()
                                .GetFirstAsync(x => x.Id == PurchaseDemandD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.DemandQty = PurchaseDemandD.DemandQty;
                        updatedetail.DepartmentId = PurchaseDemandD.DepartmentId;
                        updatedetail.ProjectId = PurchaseDemandD.ProjectId;
                        updatedetail.RequiredDate = PurchaseDemandD.RequiredDate;
                        updatedetail.Description = PurchaseDemandD.Description;
                        unitOfWork.Repository<PurchaseDemandDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _PurchaseDemandDetail = mapper.Map<PurchaseDemandDetail>(PurchaseDemandD);
                        _PurchaseDemandDetail.PurchaseDemandId = request.Id;
                        _PurchaseDemandDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _PurchaseDemandDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.PurchaseDemandDetail>().Add(_PurchaseDemandDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}