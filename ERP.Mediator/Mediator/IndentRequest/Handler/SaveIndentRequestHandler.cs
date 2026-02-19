using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IndentRequest.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class SaveIndentRequestHandler : IRequestHandler<SaveIndentRequestCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveIndentRequestHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveIndentRequestCommand, long>.Handle(SaveIndentRequestCommand request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (IndentRequest == null)
            {
                string _IndentRequestCode = "";
                if (await unitOfWork.Repository<Entities.Models.IndentRequest>().GetExistsAsync(y => y.Department.CompanyId == sessionProvider.Session.CompanyId))
                {
                    Func<IQueryable<Entities.Models.IndentRequest>, IOrderedQueryable<Entities.Models.IndentRequest>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var IndentRequestCode = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetOneAsync(y => y.IsActive == true && y.Department.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                    int No = Convert.ToInt32(IndentRequestCode.Code) + 1;
                    _IndentRequestCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _IndentRequestCode = "0000001";
                request.Code = _IndentRequestCode;

                var _IndentRequest = mapper.Map<Entities.Models.IndentRequest>(request);
                _IndentRequest.DepartmentId = sessionProvider.Session.DepartmentId;
                _IndentRequest.CreatedById = sessionProvider.Session.LoggedInUserId;
                _IndentRequest.CreatedDate = DateTime.Now;
                _IndentRequest.StatusId = 1;

                _IndentRequest.IndentRequestDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                });

                unitOfWork.Repository<Entities.Models.IndentRequest>().Add(_IndentRequest);
                SaveChanges();
                return _IndentRequest.Id;
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.IndentRequestDetail;
                masterupdate.IndentRequestDetail = null;
                var _IndentRequest = mapper.Map<Entities.Models.IndentRequest>(masterupdate);
                _IndentRequest.Code = IndentRequest.Code;
                _IndentRequest.StatusId = IndentRequest.StatusId;
                _IndentRequest.CreatedById = IndentRequest.CreatedById;
                _IndentRequest.DepartmentId = IndentRequest.DepartmentId;
                _IndentRequest.CreatedDate = IndentRequest.CreatedDate;
                _IndentRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _IndentRequest.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.IndentRequest>().Update(_IndentRequest);

                var CategoryStoreList = await unitOfWork.Repository<IndentRequestDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.IndentRequestId == request.Id && y.IsActive == true,
                    null, null, null, null, null).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    IndentRequestDetail _IndentRequestDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_IndentRequestDetail != null)
                    {
                        _IndentRequestDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _IndentRequestDetail.DeleteDate = DateTime.Now;
                        _IndentRequestDetail.IsActive = false; // Soft delete
                        _IndentRequestDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.IndentRequestDetail>().Update(_IndentRequestDetail);
                    }
                }

                // Handle additions
                foreach (var indentRequestD in detailupdate)
                {
                    if (indentRequestD.Id != 0)
                    {
                        var updatedetail = await unitOfWork.Repository<IndentRequestDetail>()
                                .GetFirstAsync(x => x.Id == indentRequestD.Id);

                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        updatedetail.ItemId = indentRequestD.ItemId;
                        updatedetail.Required = indentRequestD.Required;
                        updatedetail.Description = indentRequestD.Description;
                        unitOfWork.Repository<IndentRequestDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _IndentRequestDetail = mapper.Map<IndentRequestDetail>(indentRequestD);
                        _IndentRequestDetail.IndentRequestId = request.Id;
                        _IndentRequestDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _IndentRequestDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.IndentRequestDetail>().Add(_IndentRequestDetail);
                    }
                }

                SaveChanges();
                return _IndentRequest.Id;
            }
        }
    }
}