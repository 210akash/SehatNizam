using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.GRN.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class SaveGRNHandler : IRequestHandler<SaveGRNCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SaveGRNHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveGRNCommand, long>.Handle(SaveGRNCommand request, CancellationToken cancellationToken)
        {
            var GRN = await unitOfWork.Repository<Entities.Models.GRN>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (GRN == null)
            {
                string _GRNCode = "";
                if (await unitOfWork.Repository<Entities.Models.GRN>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.GRN>, IOrderedQueryable<Entities.Models.GRN>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var GRNCode = await unitOfWork.Repository<Entities.Models.GRN>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(GRNCode.Code) + 1;
                    _GRNCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _GRNCode = "0000001";
                request.Code = _GRNCode;

                var _GRN = mapper.Map<Entities.Models.GRN>(request);
                _GRN.CreatedById = sessionProvider.Session.LoggedInUserId;
                _GRN.CreatedDate = DateTime.Now;
                _GRN.StatusId = 1;
                _GRN.InvoiceStatusId = 1;

                _GRN.GRNDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.GRN>().Add(_GRN);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.GRNDetail;
                masterupdate.GRNDetail = null;
                var _GRN = mapper.Map<Entities.Models.GRN>(masterupdate);
                _GRN.Code = GRN.Code;
                _GRN.StatusId = GRN.StatusId;
                _GRN.InvoiceStatusId = GRN.InvoiceStatusId;
                _GRN.CreatedById = GRN.CreatedById;
                _GRN.CreatedDate = GRN.CreatedDate;
                _GRN.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _GRN.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.GRN>().Update(_GRN);


                var GRNDetailList = await unitOfWork.Repository<GRNDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.GRNId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousGRNDetailIds = GRNDetailList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousGRNDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    GRNDetail _GRNDetail = GRNDetailList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_GRNDetail != null)
                    {
                        _GRNDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _GRNDetail.DeleteDate = DateTime.Now;
                        _GRNDetail.IsActive = false;
                        _GRNDetail.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.GRNDetail>().Update(_GRNDetail);
                    }
                }

                foreach (var GRND in detailupdate)
                {
                    if (GRND.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<GRNDetail>()
                           .GetFirstAsync(x => x.Id == GRND.Id);
                        updatedetail.InspectionDetailId = GRND.InspectionDetailId;
                        updatedetail.Received = GRND.Received;
                        updatedetail.CostSheetId = GRND.CostSheetId;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<GRNDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _GRNDetail = mapper.Map<GRNDetail>(GRND);
                        _GRNDetail.GRNId = request.Id;
                        _GRNDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _GRNDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<GRNDetail>().Add(_GRNDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}