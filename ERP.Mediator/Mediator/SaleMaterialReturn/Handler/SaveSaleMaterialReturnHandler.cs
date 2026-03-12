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
using ERP.Mediator.Mediator.SaleMaterialReturn.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class SaveSaleMaterialReturnHandler : IRequestHandler<SaveSaleMaterialReturnCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSaleMaterialReturnHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveSaleMaterialReturnCommand, long>.Handle(SaveSaleMaterialReturnCommand request, CancellationToken cancellationToken)
        {
            var SaleMaterialReturn = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (SaleMaterialReturn == null)
            {
                string _SaleMaterialReturnCode = "";
                if (await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetExistsAsync(y => y.IsActive))
                {
                    Func<IQueryable<Entities.Models.SaleMaterialReturn>, IOrderedQueryable<Entities.Models.SaleMaterialReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var SaleMaterialReturnCode = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(SaleMaterialReturnCode.Code) + 1;
                    _SaleMaterialReturnCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _SaleMaterialReturnCode = "0000001";
                request.Code = _SaleMaterialReturnCode;

                var _SaleMaterialReturn = mapper.Map<Entities.Models.SaleMaterialReturn>(request);
                _SaleMaterialReturn.CreatedById = sessionProvider.Session.LoggedInUserId;
                _SaleMaterialReturn.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _SaleMaterialReturn.CreatedDate = DateTime.Now;
                _SaleMaterialReturn.StatusId = 1;

                _SaleMaterialReturn.SaleMaterialReturnDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().Add(_SaleMaterialReturn);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.SaleMaterialReturnDetail;
                masterupdate.SaleMaterialReturnDetail = null;
                var _SaleMaterialReturn = mapper.Map<Entities.Models.SaleMaterialReturn>(masterupdate);
                _SaleMaterialReturn.Code = SaleMaterialReturn.Code;
                _SaleMaterialReturn.StatusId = SaleMaterialReturn.StatusId;
                _SaleMaterialReturn.CreatedById = SaleMaterialReturn.CreatedById;
                _SaleMaterialReturn.CreatedDate = SaleMaterialReturn.CreatedDate;
                _SaleMaterialReturn.ProjectId = SaleMaterialReturn.ProjectId;
                _SaleMaterialReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _SaleMaterialReturn.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().Update(_SaleMaterialReturn);


                var SaleMaterialReturnDetailsList = await unitOfWork.Repository<SaleMaterialReturnDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.SaleMaterialReturnId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousSaleMaterialReturnDetailIds = SaleMaterialReturnDetailsList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousSaleMaterialReturnDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    SaleMaterialReturnDetail _SaleMaterialReturnDetails = SaleMaterialReturnDetailsList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_SaleMaterialReturnDetails != null)
                    {
                        _SaleMaterialReturnDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _SaleMaterialReturnDetails.DeleteDate = DateTime.Now;
                        _SaleMaterialReturnDetails.IsActive = false;
                        _SaleMaterialReturnDetails.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.SaleMaterialReturnDetail>().Update(_SaleMaterialReturnDetails);
                    }
                }

                foreach (var SaleMaterialReturnD in detailupdate)
                {
                    if (SaleMaterialReturnD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<SaleMaterialReturnDetail>()
                           .GetFirstAsync(x => x.Id == SaleMaterialReturnD.Id);
                        updatedetail.SaleMaterialDetailId = SaleMaterialReturnD.SaleMaterialDetailId;
                        updatedetail.Quantity = SaleMaterialReturnD.Quantity;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<SaleMaterialReturnDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _SaleMaterialReturnDetails = mapper.Map<SaleMaterialReturnDetail>(SaleMaterialReturnD);
                        _SaleMaterialReturnDetails.SaleMaterialReturnId = request.Id;
                        _SaleMaterialReturnDetails.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _SaleMaterialReturnDetails.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<SaleMaterialReturnDetail>().Add(_SaleMaterialReturnDetails);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}