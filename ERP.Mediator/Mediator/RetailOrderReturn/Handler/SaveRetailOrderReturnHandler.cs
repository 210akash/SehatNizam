using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrderReturn.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class SaveRetailOrderReturnHandler : IRequestHandler<SaveRetailOrderReturnCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRetailOrderReturnHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRetailOrderReturnCommand, long>.Handle(SaveRetailOrderReturnCommand request, CancellationToken cancellationToken)
        {
            var RetailOrderReturn = await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (RetailOrderReturn == null)
            {
                string _RetailOrderReturnCode = "";
                if (await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetExistsAsync(y=>y.IsActive))
                {
                    Func<IQueryable<Entities.Models.RetailOrderReturn>, IOrderedQueryable<Entities.Models.RetailOrderReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var RetailOrderReturnCode = await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(RetailOrderReturnCode.Code) + 1;
                    _RetailOrderReturnCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _RetailOrderReturnCode = "0000001";
                request.Code = _RetailOrderReturnCode;

                var _RetailOrderReturn = mapper.Map<Entities.Models.RetailOrderReturn>(request);
                _RetailOrderReturn.CreatedById = sessionProvider.Session.LoggedInUserId;
                _RetailOrderReturn.CreatedDate = DateTime.Now;
                _RetailOrderReturn.StatusId = 1;

                _RetailOrderReturn.RetailOrderReturnDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.RetailOrderReturn>().Add(_RetailOrderReturn);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.RetailOrderReturnDetail;
                masterupdate.RetailOrderReturnDetail = null;
                var _RetailOrderReturn = mapper.Map<Entities.Models.RetailOrderReturn>(masterupdate);
                _RetailOrderReturn.Code = RetailOrderReturn.Code;
                _RetailOrderReturn.StatusId = RetailOrderReturn.StatusId;
                _RetailOrderReturn.CreatedById = RetailOrderReturn.CreatedById;
                _RetailOrderReturn.CreatedDate = RetailOrderReturn.CreatedDate;
                _RetailOrderReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _RetailOrderReturn.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.RetailOrderReturn>().Update(_RetailOrderReturn);


                var RetailOrderReturnDetailsList = await unitOfWork.Repository<RetailOrderReturnDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.RetailOrderReturnId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousRetailOrderReturnDetailIds = RetailOrderReturnDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousRetailOrderReturnDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    RetailOrderReturnDetail _RetailOrderReturnDetails = RetailOrderReturnDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_RetailOrderReturnDetails != null)
                    {
                        _RetailOrderReturnDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _RetailOrderReturnDetails.DeleteDate = DateTime.Now;
                        _RetailOrderReturnDetails.IsActive = false;
                        _RetailOrderReturnDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.RetailOrderReturnDetail>().Update(_RetailOrderReturnDetails);
                    }
                }

                foreach (var RetailOrderReturnD in detailupdate)
                {
                    if (RetailOrderReturnD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<RetailOrderReturnDetail>()
                           .GetFirstAsync(x => x.Id == RetailOrderReturnD.Id);
                        updatedetail.RetailOrderReturnId = RetailOrderReturnD.RetailOrderReturnId;
                        updatedetail.Quantity = RetailOrderReturnD.Quantity;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<RetailOrderReturnDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _RetailOrderReturnDetails = mapper.Map<RetailOrderReturnDetail>(RetailOrderReturnD);
                        _RetailOrderReturnDetails.RetailOrderReturnId = request.Id;
                        _RetailOrderReturnDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _RetailOrderReturnDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<RetailOrderReturnDetail>().Add(_RetailOrderReturnDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}