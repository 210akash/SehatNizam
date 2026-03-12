using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ComparativeStatement.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class SaveComparativeStatementHandler : IRequestHandler<SaveComparativeStatementCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SaveComparativeStatementHandler(IMediator mediator,IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        async Task<long> IRequestHandler<SaveComparativeStatementCommand, long>.Handle(SaveComparativeStatementCommand request, CancellationToken cancellationToken)
        {
            var ComparativeStatement = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (ComparativeStatement == null)
            {
                string _ComparativeStatementCode = "";
                if (await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetExistsAsync())
                {
                    Func<IQueryable<Entities.Models.ComparativeStatement>, IOrderedQueryable<Entities.Models.ComparativeStatement>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var ComparativeStatementCode = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetOneAsync(y => y.IsActive == true , OrderByDesc, null);
                    int No = Convert.ToInt32(ComparativeStatementCode.Code) + 1;
                    _ComparativeStatementCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _ComparativeStatementCode = "0000001";
                request.Code = _ComparativeStatementCode;

                var _ComparativeStatement = mapper.Map<Entities.Models.ComparativeStatement>(request);
                _ComparativeStatement.CreatedById = sessionProvider.Session.LoggedInUserId;
                _ComparativeStatement.CreatedDate = DateTime.Now;
                _ComparativeStatement.StatusId = 1;

                _ComparativeStatement.ComparativeStatementDetail.ForEach(y =>
                {
                    y.CreatedDate = DateTime.Now;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value

                    y.ComparativeStatementVendor.ForEach(x => {
                        x.CreatedDate = DateTime.Now;
                        x.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                    });
                });

                unitOfWork.Repository<Entities.Models.ComparativeStatement>().Add(_ComparativeStatement);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate =  masterupdate.ComparativeStatementDetail;
                masterupdate.ComparativeStatementDetail = null;
                var _ComparativeStatement = mapper.Map<Entities.Models.ComparativeStatement>(masterupdate);
                _ComparativeStatement.Code = ComparativeStatement.Code;
                _ComparativeStatement.StatusId = ComparativeStatement.StatusId;
                _ComparativeStatement.CreatedById = ComparativeStatement.CreatedById;
                _ComparativeStatement.CreatedDate = ComparativeStatement.CreatedDate;
                _ComparativeStatement.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _ComparativeStatement.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.ComparativeStatement>().Update(_ComparativeStatement);

                Expression<Func<ComparativeStatementDetail, object>>[] includes = {
                x => x.ComparativeStatementVendor
            };

                var CategoryStoreList = await unitOfWork.Repository<ComparativeStatementDetail>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.ComparativeStatementId == request.Id && y.IsActive == true,
                    null, null, null, null, includes).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y=>y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    ComparativeStatementDetail _ComparativeStatementDetail = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_ComparativeStatementDetail != null)
                    {
                        foreach (var _ComparativeStatementVendor in _ComparativeStatementDetail.ComparativeStatementVendor)
                        {
                            _ComparativeStatementVendor.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _ComparativeStatementVendor.DeleteDate = DateTime.Now;
                            _ComparativeStatementVendor.IsActive = false; // Soft delete
                            _ComparativeStatementVendor.IsDelete = true; // Soft delete
                            unitOfWork.Repository<Entities.Models.ComparativeStatementVendor>().Update(_ComparativeStatementVendor);
                        }

                        _ComparativeStatementDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _ComparativeStatementDetail.DeleteDate = DateTime.Now;
                        _ComparativeStatementDetail.IsActive = false; // Soft delete
                        _ComparativeStatementDetail.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.ComparativeStatementDetail>().Update(_ComparativeStatementDetail);
                    }
                }

                // Handle additions and updates
                foreach (var ComparativeStatementD in detailupdate)
                {
                    // updates
                    if (ComparativeStatementD.Id != 0)
                    {
                        var ComparativeStatementVendorList = await unitOfWork.Repository<ComparativeStatementVendor>()
                              .GetPagingWhereAsNoTrackingAsync(y => y.ComparativeStatementDetailId == ComparativeStatementD.Id && y.IsActive == true,
                              null, null, null, null, null).Item1.ToListAsync();

                        List<long> previousvendorIds = ComparativeStatementVendorList
                            .Select(y => y.Id)
                            .ToList();

                        List<long> currentvendorIds = ComparativeStatementD.ComparativeStatementVendor.Select(y => y.Id).ToList();
                        List<long> deletedvendorIds = previousvendorIds.Except(currentvendorIds).ToList();

                        //Delete vendor Price
                        foreach (var vendorid in deletedvendorIds)
                        {
                            var deletecsvendor = await unitOfWork.Repository<ComparativeStatementVendor>().GetFirstAsync(y => y.Id == vendorid);
                            deletecsvendor.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            deletecsvendor.ModifiedDate = DateTime.Now;
                            deletecsvendor.DeleteDate = DateTime.Now;
                            deletecsvendor.IsDelete = true;
                            deletecsvendor.IsActive = false;
                            unitOfWork.Repository<ComparativeStatementVendor>().Update(deletecsvendor);
                        }

                        foreach (var ComparativeStatementVendor in ComparativeStatementD.ComparativeStatementVendor)
                        {
                            if (ComparativeStatementVendor.Id != 0)
                            {
                                var _ComparativeStatementVendor = mapper.Map<ComparativeStatementVendor>(ComparativeStatementVendor);
                                _ComparativeStatementVendor.ComparativeStatementDetailId = ComparativeStatementVendor.ComparativeStatementDetailId;
                                _ComparativeStatementVendor.ModifiedById = sessionProvider.Session.LoggedInUserId;
                                _ComparativeStatementVendor.ModifiedDate = DateTime.Now;
                                unitOfWork.Repository<ComparativeStatementVendor>().Update(_ComparativeStatementVendor);
                            }
                            else
                            {
                                var addvendorStatement = mapper.Map<ComparativeStatementVendor>(ComparativeStatementVendor);
                                addvendorStatement.ComparativeStatementDetailId = ComparativeStatementD.Id;
                                addvendorStatement.CreatedById = sessionProvider.Session.LoggedInUserId;
                                addvendorStatement.CreatedDate = DateTime.Now;
                                unitOfWork.Repository<ComparativeStatementVendor>().Add(addvendorStatement);
                            }
                        }

                        var updatedetail = await unitOfWork.Repository<ComparativeStatementDetail>()
                           .GetFirstAsync(x => x.Id == ComparativeStatementD.Id, null, null, "ComparativeStatementVendor");
                        updatedetail.PurchaseDemandDetailId = ComparativeStatementD.PurchaseDemandDetailId;
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<ComparativeStatementDetail>().Update(updatedetail);
                    }
                    else
                    {
                        var _ComparativeStatementDetail = mapper.Map<ComparativeStatementDetail>(ComparativeStatementD);
                        _ComparativeStatementDetail.ComparativeStatementId = request.Id;
                        _ComparativeStatementDetail.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _ComparativeStatementDetail.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<ComparativeStatementDetail>().Add(_ComparativeStatementDetail);
                    }
                }

                SaveChanges();
            }
            return 200;
        }
    }
}