using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Inspection.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class SaveInspectionHandler : IRequestHandler<SaveInspectionCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SaveInspectionHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            try
            {
                return unitOfWork.SaveChanges();

            }
            catch (Exception re)
            {

                throw;
            }
        }

        async Task<long> IRequestHandler<SaveInspectionCommand, long>.Handle(SaveInspectionCommand request, CancellationToken cancellationToken)
        {
            var Inspection = await unitOfWork.Repository<Entities.Models.Inspection>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (Inspection == null)
            {
                string _InspectionCode = "";
                if (await unitOfWork.Repository<Entities.Models.Inspection>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.Inspection>, IOrderedQueryable<Entities.Models.Inspection>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var InspectionCode = await unitOfWork.Repository<Entities.Models.Inspection>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(InspectionCode.Code) + 1;
                    _InspectionCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _InspectionCode = "0000001";
                request.Code = _InspectionCode;

                var _Inspection = mapper.Map<Entities.Models.Inspection>(request);
                _Inspection.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Inspection.CreatedDate = DateTime.Now;
                _Inspection.StatusId = 1;

                _Inspection.InspectionDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                });

                unitOfWork.Repository<Entities.Models.Inspection>().Add(_Inspection);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.InspectionDetail;
                masterupdate.InspectionDetail = null;
                var _Inspection = mapper.Map<Entities.Models.Inspection>(masterupdate);
                _Inspection.Code = Inspection.Code;
                _Inspection.StatusId = Inspection.StatusId;
                _Inspection.CreatedById = Inspection.CreatedById;
                _Inspection.CreatedDate = Inspection.CreatedDate;
                _Inspection.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Inspection.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Inspection>().Update(_Inspection);


                var InspectionDetailList = await unitOfWork.Repository<InspectionDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.InspectionId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousInspectionDetailIds = InspectionDetailList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousInspectionDetailIds.Except(currentCategoryStoreIds).ToList();

                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    InspectionDetail _InspectionDetail = InspectionDetailList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_InspectionDetail != null)
                    {
                        _InspectionDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _InspectionDetail.DeleteDate = DateTime.Now;
                        _InspectionDetail.IsActive = false;
                        _InspectionDetail.IsDelete = true;
                        unitOfWork.Repository<Entities.Models.InspectionDetail>().Update(_InspectionDetail);
                    }
                }

                foreach (var InspectionD in detailupdate)
                {
                    if (InspectionD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<InspectionDetail>()
                           .GetFirstAsync(x => x.Id == InspectionD.Id);
                        updatedetail.IGPDetailId = InspectionD.IGPDetailId;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.Rejected = InspectionD.Rejected;
                        updatedetail.RejectReasonId = InspectionD.RejectReasonId;
                        updatedetail.Remarks = InspectionD.Remarks;
                        unitOfWork.Repository<InspectionDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _InspectionDetail = mapper.Map<InspectionDetail>(InspectionD);
                        _InspectionDetail.InspectionId = request.Id;
                        _InspectionDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _InspectionDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<InspectionDetail>().Add(_InspectionDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}