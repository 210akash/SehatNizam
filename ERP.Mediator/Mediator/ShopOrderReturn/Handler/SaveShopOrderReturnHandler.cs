using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShopOrderReturn.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class SaveShopOrderReturnHandler : IRequestHandler<SaveShopOrderReturnCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveShopOrderReturnHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveShopOrderReturnCommand, long>.Handle(SaveShopOrderReturnCommand request, CancellationToken cancellationToken)
        {
            var ShopOrderReturn = await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (ShopOrderReturn == null)
            {
                string _ShopOrderReturnCode = "";
                if (await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetExistsAsync(y=>y.IsActive))
                {
                    Func<IQueryable<Entities.Models.ShopOrderReturn>, IOrderedQueryable<Entities.Models.ShopOrderReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var ShopOrderReturnCode = await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(ShopOrderReturnCode.Code) + 1;
                    _ShopOrderReturnCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _ShopOrderReturnCode = "0000001";
                request.Code = _ShopOrderReturnCode;

                var _ShopOrderReturn = mapper.Map<Entities.Models.ShopOrderReturn>(request);
                _ShopOrderReturn.CreatedById = sessionProvider.Session.LoggedInUserId;
                _ShopOrderReturn.CreatedDate = DateTime.Now;
                _ShopOrderReturn.StatusId = 1;

                _ShopOrderReturn.ShopOrderReturnDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.ShopOrderReturn>().Add(_ShopOrderReturn);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.ShopOrderReturnDetail;
                masterupdate.ShopOrderReturnDetail = null;
                var _ShopOrderReturn = mapper.Map<Entities.Models.ShopOrderReturn>(masterupdate);
                _ShopOrderReturn.Code = ShopOrderReturn.Code;
                _ShopOrderReturn.StatusId = ShopOrderReturn.StatusId;
                _ShopOrderReturn.CreatedById = ShopOrderReturn.CreatedById;
                _ShopOrderReturn.CreatedDate = ShopOrderReturn.CreatedDate;
                _ShopOrderReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _ShopOrderReturn.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.ShopOrderReturn>().Update(_ShopOrderReturn);


                var ShopOrderReturnDetailsList = await unitOfWork.Repository<ShopOrderReturnDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.ShopOrderReturnId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousShopOrderReturnDetailIds = ShopOrderReturnDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousShopOrderReturnDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    ShopOrderReturnDetail _ShopOrderReturnDetails = ShopOrderReturnDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_ShopOrderReturnDetails != null)
                    {
                        _ShopOrderReturnDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _ShopOrderReturnDetails.DeleteDate = DateTime.Now;
                        _ShopOrderReturnDetails.IsActive = false;
                        _ShopOrderReturnDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.ShopOrderReturnDetail>().Update(_ShopOrderReturnDetails);
                    }
                }

                foreach (var ShopOrderReturnD in detailupdate)
                {
                    if (ShopOrderReturnD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<ShopOrderReturnDetail>()
                           .GetFirstAsync(x => x.Id == ShopOrderReturnD.Id);
                        updatedetail.ShopOrderReturnId = ShopOrderReturnD.ShopOrderReturnId;
                        updatedetail.Quantity = ShopOrderReturnD.Quantity;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<ShopOrderReturnDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _ShopOrderReturnDetails = mapper.Map<ShopOrderReturnDetail>(ShopOrderReturnD);
                        _ShopOrderReturnDetails.ShopOrderReturnId = request.Id;
                        _ShopOrderReturnDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _ShopOrderReturnDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<ShopOrderReturnDetail>().Add(_ShopOrderReturnDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}