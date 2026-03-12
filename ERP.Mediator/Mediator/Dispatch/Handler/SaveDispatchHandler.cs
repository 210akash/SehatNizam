using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Dispatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class SaveDispatchHandler : IRequestHandler<SaveDispatchCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;
        private long DNNumber = 0;

        public SaveDispatchHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        async Task<long> IRequestHandler<SaveDispatchCommand, long>.Handle(SaveDispatchCommand request, CancellationToken cancellationToken)
        {
            var Dispatch = await unitOfWork.Repository<Entities.Models.Dispatch>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (Dispatch == null)
            {
                string _DispatchCode = "";
                var check = await unitOfWork.Repository<Entities.Models.Dispatch>().GetExistsAsync(x => x.IsActive);
                if (check)
                {
                    Func<IQueryable<Entities.Models.Dispatch>, IOrderedQueryable<Entities.Models.Dispatch>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                    var DispatchCode = await unitOfWork.Repository<Entities.Models.Dispatch>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                    int No = Convert.ToInt32(DispatchCode.Code) + 1;
                    _DispatchCode = No.ToString().PadLeft(7, '0');
                }
                else
                    _DispatchCode = "0000001";
                request.Code = _DispatchCode;

                var _Dispatch = mapper.Map<Entities.Models.Dispatch>(request);
                _Dispatch.CreatedById = sessionProvider.Session.LoggedInUserId;
                _Dispatch.ProjectId = sessionProvider.Session.SelectedWarehouseId;
                _Dispatch.StatusId = 1;

                foreach (var y in _Dispatch.DispatchOrder)
                {
                    // 1. Fill header fields
                    y.DCCode = await generateDelieveryChallanCode(y.OrderId, check);
                    y.INVCode = await generateInvoiceCode(y.OrderId, check);
                    y.StatusId = (long)OrderStatusEnum.Create;
                    y.CreatedById = sessionProvider.Session.LoggedInUserId;
                    y.CreatedDate = DateTime.Now;

                    // make sure the running totals start from 0
                    y.DistributorAmount = 0;
                    y.TradePromo = 0;
                    y.TradeMargin = 0;
                    y.DistributorMargin = 0;

                    // 2. Loop through the details
                    foreach (var detail in y.DispatchDetail)
                    {
                        var orderItem = await unitOfWork.Repository<OrderItems>()
                            .GetFirstAsync(o => o.Id == detail.OrderItemId);

                        y.DistributorAmount += orderItem.DistributorPrice * detail.Quantity;
                        y.TradePromo += orderItem.DistributorPromo.GetValueOrDefault() * detail.Quantity;
                        y.TradeMargin += (orderItem.RetailPrice.GetValueOrDefault()
                                               - orderItem.TradePrice
                                               - orderItem.DistributorPromo.GetValueOrDefault()) * detail.Quantity;
                        y.DistributorMargin += (orderItem.TradePrice - orderItem.DistributorPrice) * detail.Quantity;

                        detail.CreatedDate = DateTime.Now;
                        detail.CreatedById = sessionProvider.Session.LoggedInUserId; // Or any desired value
                    }
                }

                unitOfWork.Repository<Entities.Models.Dispatch>().Add(_Dispatch);
                SaveChanges();
            }
            else
            {
                var masterupdate = request;
                var detailupdate = masterupdate.DispatchOrder;
                masterupdate.DispatchOrder = null;
                var _Dispatch = mapper.Map<Entities.Models.Dispatch>(masterupdate);
                _Dispatch.Code = Dispatch.Code;
                _Dispatch.StatusId = Dispatch.StatusId;
                _Dispatch.CreatedById = Dispatch.CreatedById;
                _Dispatch.ProjectId = Dispatch.ProjectId;
                _Dispatch.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _Dispatch.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Dispatch>().Update(_Dispatch);

                Expression<Func<ERP.Entities.Models.DispatchOrder, object>>[] includes = {
                x => x.DispatchDetail
            };

                var CategoryStoreList = await unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>()
                    .GetPagingWhereAsNoTrackingAsync(y => y.DispatchId == request.Id && y.IsActive == true,
                    null, null, null, null, includes).Item1.ToListAsync();

                List<long> previousCategoryStoreIds = CategoryStoreList
                    .Select(y => y.Id)
                    .ToList();

                List<long> currentCategoryStoreIds = detailupdate.Select(y => y.Id).ToList();
                List<long> deletedCategoryStoreIds = previousCategoryStoreIds.Except(currentCategoryStoreIds).ToList();

                // Handle deletions
                foreach (var deletedCategoryStoreId in deletedCategoryStoreIds)
                {
                    ERP.Entities.Models.DispatchOrder _DispatchOrder = CategoryStoreList.Where(y => y.Id == deletedCategoryStoreId).FirstOrDefault();

                    if (_DispatchOrder != null)
                    {
                        foreach (var _DispatchDetail in _DispatchOrder.DispatchDetail)
                        {
                            _DispatchDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _DispatchDetail.DeleteDate = DateTime.Now;
                            _DispatchDetail.IsActive = false; // Soft delete
                            _DispatchDetail.IsDelete = true; // Soft delete
                            unitOfWork.Repository<Entities.Models.DispatchDetail>().Update(_DispatchDetail);
                        }

                        _DispatchOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _DispatchOrder.DeleteDate = DateTime.Now;
                        _DispatchOrder.IsActive = false; // Soft delete
                        _DispatchOrder.IsDelete = true; // Soft delete
                        unitOfWork.Repository<Entities.Models.DispatchOrder>().Update(_DispatchOrder);
                    }
                }

                // Handle additions and updates
                foreach (var DispatchD in detailupdate)
                {
                   

                    // updates
                    if (DispatchD.Id != 0)
                    {
                        var DispatchDetailList = await unitOfWork.Repository<DispatchDetail>()
                              .GetPagingWhereAsNoTrackingAsync(y => y.DispatchOrderId == DispatchD.Id && y.IsActive == true,
                              null, null, null, null, null).Item1.ToListAsync();

                        List<long> previousorderIds = DispatchDetailList
                            .Select(y => y.Id)
                            .ToList();

                        List<long> currentorderIds = DispatchD.DispatchDetail.Select(y => y.Id).ToList();
                        List<long> deletedorderIds = previousorderIds.Except(currentorderIds).ToList();

                        //Delete order Price
                        foreach (var orderid in deletedorderIds)
                        {
                            var deletecsorder = await unitOfWork.Repository<DispatchDetail>().GetFirstAsync(y => y.Id == orderid);
                            deletecsorder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            deletecsorder.ModifiedDate = DateTime.Now;
                            deletecsorder.DeleteDate = DateTime.Now;
                            deletecsorder.IsDelete = true;
                            deletecsorder.IsActive = false;
                            unitOfWork.Repository<DispatchDetail>().Update(deletecsorder);
                        }

                        foreach (var DispatchDetail in DispatchD.DispatchDetail)
                        {
                            if (DispatchDetail.Id != 0)
                            {
                                var _DispatchDetail = mapper.Map<DispatchDetail>(DispatchDetail);
                                _DispatchDetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                                _DispatchDetail.ModifiedDate = DateTime.Now;
                                unitOfWork.Repository<DispatchDetail>().Update(_DispatchDetail);
                            }
                            else
                            {
                                var addorderStatement = mapper.Map<DispatchDetail>(DispatchDetail);
                                addorderStatement.DispatchOrderId = DispatchD.Id;
                                addorderStatement.CreatedById = sessionProvider.Session.LoggedInUserId;
                                addorderStatement.CreatedDate = DateTime.Now;
                                unitOfWork.Repository<DispatchDetail>().Add(addorderStatement);
                            }
                        }

                        var updatedetail = await unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>()
                           .GetFirstAsync(x => x.Id == DispatchD.Id, null, null, "DispatchDetail");
                        updatedetail.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        updatedetail.ModifiedDate = DateTime.Now;

                        updatedetail.DistributorAmount = 0;
                        updatedetail.TradePromo = 0;
                        updatedetail.TradeMargin = 0;
                        updatedetail.DistributorMargin = 0;
                        updatedetail.OrderFreightCharges = DispatchD.OrderFreightCharges;
                        foreach (var DispatchDetail in updatedetail.DispatchDetail)
                        {
                            var orderItem = await unitOfWork.Repository<OrderItems>()
                                .GetFirstAsNoTrackingAsync(x => x.Id == DispatchDetail.OrderItemId);

                            // Calculate the Distributor Amount
                            updatedetail.DistributorAmount += orderItem.DistributorPrice * DispatchDetail.Quantity;

                            // Calculate Trade Promo (assuming this calculation is correct)
                            updatedetail.TradePromo += orderItem.DistributorPromo.Value * DispatchDetail.Quantity;

                            // Corrected TradeMargin calculation
                            updatedetail.TradeMargin += (orderItem.RetailPrice.Value * DispatchDetail.Quantity) - (orderItem.TradePrice * DispatchDetail.Quantity) - (orderItem.DistributorPromo.Value * DispatchDetail.Quantity);

                            // Distributor Margin (unchanged logic)
                            updatedetail.DistributorMargin += (orderItem.TradePrice - orderItem.DistributorPrice) * DispatchDetail.Quantity;
                        }

                        unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>().Update(updatedetail);
                    }
                    else
                    {
                        var _DispatchOrder = mapper.Map<ERP.Entities.Models.DispatchOrder>(DispatchD);
                        _DispatchOrder.DCCode = await generateDelieveryChallanCode(_DispatchOrder.OrderId, true);
                        _DispatchOrder.INVCode = await generateInvoiceCode(_DispatchOrder.OrderId, true);
                        _DispatchOrder.StatusId = (long?)OrderStatusEnum.Create;
                        _DispatchOrder.DispatchId = request.Id;
                        _DispatchOrder.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _DispatchOrder.CreatedDate = DateTime.Now;

                        _DispatchOrder.DistributorAmount = 0;
                        _DispatchOrder.TradePromo = 0;
                        _DispatchOrder.TradeMargin = 0;
                        _DispatchOrder.DistributorMargin = 0;

                        foreach (var DispatchDetail in _DispatchOrder.DispatchDetail)
                        {
                            var orderItem = await unitOfWork.Repository<OrderItems>()
                                .GetFirstAsNoTrackingAsync(x => x.Id == DispatchDetail.OrderItemId);

                            // Calculate the Distributor Amount
                            _DispatchOrder.DistributorAmount += orderItem.DistributorPrice * DispatchDetail.Quantity;

                            // Calculate Trade Promo (assuming this calculation is correct)
                            _DispatchOrder.TradePromo += orderItem.DistributorPromo.Value * DispatchDetail.Quantity;

                            // Corrected TradeMargin calculation
                            _DispatchOrder.TradeMargin += (orderItem.RetailPrice.Value * DispatchDetail.Quantity) - (orderItem.TradePrice * DispatchDetail.Quantity) - (orderItem.DistributorPromo.Value * DispatchDetail.Quantity);

                            // Distributor Margin (unchanged logic)
                            _DispatchOrder.DistributorMargin += (orderItem.TradePrice - orderItem.DistributorPrice) * DispatchDetail.Quantity;
                        }

                        unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>().Add(_DispatchOrder);
                    }
                }

                SaveChanges();
            }
            return 200;
        }

        public async Task<string> generateDelieveryChallanCode(long orderId, bool check)
        {
            string newNo = "";
            if (check)
            {
                if(DNNumber == 0)
                {
                    DNNumber = unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>().GetAllAsync().Result.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                }
                
                newNo = "KCD-" + orderId + "-" + DNNumber;
            }
            else
            {
                newNo = "KCD-" + orderId + "-1";
            }
            return newNo;
        }

        public async Task<string> generateInvoiceCode(long orderId, bool check)
        {
            string newNo = "";
            if (check)
            {
                newNo = "KC-SINV-" + orderId + "-" + DNNumber;
                DNNumber++;
            }
            else
            {
                newNo = "KC-SINV-" + orderId + "-1";
            }
            return newNo;
        }
    }
}