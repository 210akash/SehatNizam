using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Issuance.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class SaveIssuanceHandler : IRequestHandler<SaveIssuanceCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveIssuanceHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveIssuanceCommand, long>.Handle(SaveIssuanceCommand request, CancellationToken cancellationToken)
        {
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (Issuance == null)
            {
                string _IssuanceCode = "";
                if (await unitOfWork.Repository<Entities.Models.Issuance>().GetExistsAsync(y=>y.IsActive))
                {
                    Func<IQueryable<Entities.Models.Issuance>, IOrderedQueryable<Entities.Models.Issuance>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var IssuanceCode = await unitOfWork.Repository<Entities.Models.Issuance>().GetOneAsync(y => y.IsActive == true , OrderByDesc, null);
                    int No = Convert.ToInt32(IssuanceCode.Code) + 1;
                    _IssuanceCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _IssuanceCode = "0000001";
                request.Code = _IssuanceCode;

                var _Issuance = mapper.Map<Entities.Models.Issuance>(request);
                _Issuance.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Issuance.CreatedDate = DateTime.Now;
                _Issuance.StatusId = 1;
                _Issuance.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _Issuance.IssuanceDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.Issuance>().Add(_Issuance);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.IssuanceDetail;
                masterupdate.IssuanceDetail = null;
                var _Issuance = mapper.Map<Entities.Models.Issuance>(masterupdate);
                _Issuance.Code = Issuance.Code;
                _Issuance.StatusId = Issuance.StatusId;
                _Issuance.CreatedById = Issuance.CreatedById;
                _Issuance.CreatedDate = Issuance.CreatedDate;
                _Issuance.ProjectId = Issuance.ProjectId;
                _Issuance.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Issuance.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Issuance>().Update(_Issuance);

                var CategoryStoreList = await unitOfWork.Repository<IssuanceDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.IssuanceId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList.Select(y => y.Id).ToList();
                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    IssuanceDetail _IssuanceDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_IssuanceDetail != null)
                    {
                        _IssuanceDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _IssuanceDetail.DeleteDate = DateTime.Now;
                        _IssuanceDetail.IsActive = false; // Soft delete
                        _IssuanceDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.IssuanceDetail>().Update(_IssuanceDetail);
                    }
                }

                // Handle additions
                foreach (var IssuanceD in detailupdate)
                {
                    if (IssuanceD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<IssuanceDetail>().GetFirstAsync(x => x.Id == IssuanceD.Id);
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.Quantity = IssuanceD.Quantity;
                        updatedetail.Rate = IssuanceD.Rate;
                        updatedetail.CostSheetId = IssuanceD.CostSheetId;
                        unitOfWork.Repository<IssuanceDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _IssuanceDetail = mapper.Map<IssuanceDetail>(IssuanceD);
                        _IssuanceDetail.IssuanceId = request.Id;
                        _IssuanceDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _IssuanceDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.IssuanceDetail>().Add(_IssuanceDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}