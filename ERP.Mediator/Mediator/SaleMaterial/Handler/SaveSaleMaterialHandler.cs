using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleMaterial.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class SaveSaleMaterialHandler : IRequestHandler<SaveSaleMaterialCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSaleMaterialHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSaleMaterialCommand, long>.Handle(SaveSaleMaterialCommand request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (SaleMaterial == null)
            {
                string _SaleMaterialCode = "";
                if (await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId))
                {
                    Func<IQueryable<Entities.Models.SaleMaterial>, IOrderedQueryable<Entities.Models.SaleMaterial>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var SaleMaterialCode = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                    int No = Convert.ToInt32(SaleMaterialCode.Code) + 1;
                    _SaleMaterialCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _SaleMaterialCode = "0000001";
                request.Code = _SaleMaterialCode;

                var _SaleMaterial = mapper.Map<Entities.Models.SaleMaterial>(request);
                _SaleMaterial.CompanyId = sessionProvider.Session.CompanyId;
                _SaleMaterial.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _SaleMaterial.CreatedById = sessionProvider.Session.LoggedInUserId;
                _SaleMaterial.CreatedDate = DateTime.Now;
                _SaleMaterial.StatusId = 1;

                _SaleMaterial.SaleMaterialDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.SaleMaterial>().Add(_SaleMaterial);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.SaleMaterialDetail;
                masterupdate.SaleMaterialDetail = null;
                var _SaleMaterial = mapper.Map<Entities.Models.SaleMaterial>(masterupdate);
                _SaleMaterial.StatusId = SaleMaterial.StatusId;
                _SaleMaterial.CreatedById = SaleMaterial.CreatedById;
                _SaleMaterial.CompanyId = SaleMaterial.CompanyId;
                _SaleMaterial.CreatedDate = SaleMaterial.CreatedDate;
                _SaleMaterial.ProjectId = SaleMaterial.ProjectId;
                _SaleMaterial.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _SaleMaterial.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.SaleMaterial>().Update(_SaleMaterial);

                var CategoryStoreList = await unitOfWork.Repository<SaleMaterialDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.SaleMaterialId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    SaleMaterialDetail _SaleMaterialDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_SaleMaterialDetail != null)
                    {
                        _SaleMaterialDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _SaleMaterialDetail.DeleteDate = DateTime.Now;
                        _SaleMaterialDetail.IsActive = false; // Soft delete
                        _SaleMaterialDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<SaleMaterialDetail>().Update(_SaleMaterialDetail);
                    }
                }

                // Handle additions
                foreach (var SaleMaterialD in detailupdate)
                {
                    if (SaleMaterialD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<SaleMaterialDetail>()
                                .GetFirstAsync(x => x.Id == SaleMaterialD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.ItemId = SaleMaterialD.ItemId;
                        updatedetail.Quantity = SaleMaterialD.Quantity;
                        unitOfWork.Repository<SaleMaterialDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _SaleMaterialDetail = mapper.Map<SaleMaterialDetail>(SaleMaterialD);
                        _SaleMaterialDetail.SaleMaterialId = request.Id;
                        _SaleMaterialDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _SaleMaterialDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<SaleMaterialDetail>().Add(_SaleMaterialDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}