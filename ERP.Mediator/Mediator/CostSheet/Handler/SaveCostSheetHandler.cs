using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.CostSheet.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.CostSheet.Handler
{
    public class SaveCostSheetHandler : IRequestHandler<SaveCostSheetCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveCostSheetHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveCostSheetCommand, long>.Handle(SaveCostSheetCommand request, CancellationToken cancellationToken)
        {
            var CostSheet = await unitOfWork.Repository<Entities.Models.CostSheet>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (CostSheet == null)
            {
                //string _CostSheetCode = "";
                //if (await unitOfWork.Repository<Entities.Models.CostSheet>().GetExistsAsync(y => y.Item.CompanyId == sessionProvider.Session.CompanyId))
                //{
                //    Func<IQueryable<Entities.Models.CostSheet>, IOrderedQueryable<Entities.Models.CostSheet>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                //    var CostSheetCode = await unitOfWork.Repository<Entities.Models.CostSheet>().GetOneAsync(y => y.IsActive == true && y.Item.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                //    int No = Convert.ToInt32(CostSheetCode.Code) + 1;
                //    _CostSheetCode = No.ToString().PadLeft(7, '0');
                //}
                //else
                //    _CostSheetCode = "0000001";
                //request.Code = _CostSheetCode;

                var _CostSheet = mapper.Map<Entities.Models.CostSheet>(request);
                _CostSheet.CreatedById = sessionProvider.Session.LoggedInUserId;
                _CostSheet.StatusId = 1;

                _CostSheet.CostSheetDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.CostSheet>().Add(_CostSheet);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.CostSheetDetail;
                masterupdate.CostSheetDetail = null;
                var _CostSheet = mapper.Map<Entities.Models.CostSheet>(masterupdate);
                _CostSheet.StatusId = CostSheet.StatusId;
                _CostSheet.CreatedById = CostSheet.CreatedById;
                _CostSheet.CreatedDate = CostSheet.CreatedDate;
                _CostSheet.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _CostSheet.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.CostSheet>().Update(_CostSheet);

                var CategoryStoreList = await unitOfWork.Repository<CostSheetDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.CostSheetId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    CostSheetDetail _CostSheetDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_CostSheetDetail != null)
                    {
                        _CostSheetDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _CostSheetDetail.DeleteDate = DateTime.Now;
                        _CostSheetDetail.IsActive = false; // Soft delete
                        _CostSheetDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.CostSheetDetail>().Update(_CostSheetDetail);
                    }
                }

                // Handle additions
                foreach (var CostSheetD in detailupdate)
                {
                    if (CostSheetD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<CostSheetDetail>()
                                .GetFirstAsync(x => x.Id == CostSheetD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.ItemId = CostSheetD.ItemId;
                        updatedetail.Quantity = CostSheetD.Quantity;
                        unitOfWork.Repository<CostSheetDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _CostSheetDetail = mapper.Map<CostSheetDetail>(CostSheetD);
                        _CostSheetDetail.CostSheetId = request.Id;
                        _CostSheetDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _CostSheetDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.CostSheetDetail>().Add(_CostSheetDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}