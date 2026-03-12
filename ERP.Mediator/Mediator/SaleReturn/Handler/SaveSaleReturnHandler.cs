using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.SaleReturn.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class SaveSaleReturnHandler : IRequestHandler<SaveSaleReturnCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSaleReturnHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSaleReturnCommand, long>.Handle(SaveSaleReturnCommand request, CancellationToken cancellationToken)
        {
            var SaleReturn = await unitOfWork.Repository<Entities.Models.SaleReturn>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (SaleReturn == null)
            {
                string _SaleReturnCode = "";
                if (await unitOfWork.Repository<Entities.Models.SaleReturn>().GetExistsAsync(y => y.IsActive))
                {
                    Func<IQueryable<Entities.Models.SaleReturn>, IOrderedQueryable<Entities.Models.SaleReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var SaleReturnCode = await unitOfWork.Repository<Entities.Models.SaleReturn>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(SaleReturnCode.Code) + 1;
                    _SaleReturnCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _SaleReturnCode = "0000001";
                request.Code = _SaleReturnCode;

                var _SaleReturn = mapper.Map<Entities.Models.SaleReturn>(request);
                _SaleReturn.CreatedById = sessionProvider.Session.LoggedInUserId;
                _SaleReturn.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _SaleReturn.CreatedDate = DateTime.Now;
                _SaleReturn.StatusId = 1;

                _SaleReturn.SaleReturnDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.SaleReturn>().Add(_SaleReturn);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.SaleReturnDetail;
                masterupdate.SaleReturnDetail = null;
                var _SaleReturn = mapper.Map<Entities.Models.SaleReturn>(masterupdate);
                _SaleReturn.Code = SaleReturn.Code;
                _SaleReturn.StatusId = SaleReturn.StatusId;
                _SaleReturn.CreatedById = SaleReturn.CreatedById;
                _SaleReturn.CreatedDate = SaleReturn.CreatedDate;
                _SaleReturn.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _SaleReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _SaleReturn.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.SaleReturn>().Update(_SaleReturn);


                var SaleReturnDetailsList = await unitOfWork.Repository<SaleReturnDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.SaleReturnId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousSaleReturnDetailIds = SaleReturnDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousSaleReturnDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    SaleReturnDetail _SaleReturnDetails = SaleReturnDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_SaleReturnDetails != null)
                    {
                        _SaleReturnDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _SaleReturnDetails.DeleteDate = DateTime.Now;
                        _SaleReturnDetails.IsActive = false;
                        _SaleReturnDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.SaleReturnDetail>().Update(_SaleReturnDetails);
                    }
                }

                foreach (var SaleReturnD in detailupdate)
                {
                    if (SaleReturnD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<SaleReturnDetail>()
                           .GetFirstAsync(x => x.Id == SaleReturnD.Id);
                        updatedetail.DispatchDetailId = SaleReturnD.DispatchDetailId;
                        updatedetail.Quantity = SaleReturnD.Quantity;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<SaleReturnDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _SaleReturnDetails = mapper.Map<SaleReturnDetail>(SaleReturnD);
                        _SaleReturnDetails.SaleReturnId = request.Id;
                        _SaleReturnDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _SaleReturnDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<SaleReturnDetail>().Add(_SaleReturnDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}