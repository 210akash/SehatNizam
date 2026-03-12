using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShopOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class SaveShopOrderHandler : IRequestHandler<CreateShopOrderCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public SaveShopOrderHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<CreateShopOrderCommand, long>.Handle(CreateShopOrderCommand request, CancellationToken cancellationToken)
        {
            var shopOrder = await unitOfWork.Repository<Entities.Models.ShopOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (shopOrder == null)
            {
                var _shopOrder = mapper.Map<Entities.Models.ShopOrder>(request);
                _shopOrder.CreatedDate = DateTime.Now;
                _shopOrder.ShopOrderStatusId = (long)OrderStatusEnum.OrderCreate;

                unitOfWork.Repository<Entities.Models.ShopOrder>().Add(_shopOrder);
                SaveChanges();

                foreach (var item in request.ShopOrderItemsList)
                {
                    if (item.Quantity > 0)
                    {
                        var _shopOrderItems = mapper.Map<ShopOrderItems>(item);
                        _shopOrderItems.IsActive = true;
                        _shopOrderItems.IsDelete = false;
                        _shopOrderItems.ShopOrderId = _shopOrder.Id;
                        _shopOrderItems.CreatedById = request.CreatedById;
                        _shopOrderItems.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<ShopOrderItems>().Add(_shopOrderItems);
                    }
                }

                SaveChanges();
            }
            else
            {
                if (shopOrder == null)
                    throw new Exception("Shop order not found");

                // ---- Update ShopOrder (preserve created fields) ----
                shopOrder.ModifiedById = request.CreatedById;
                shopOrder.ModifiedDate = DateTime.Now;

                var _shopOrder = mapper.Map<Entities.Models.ShopOrder>(request);
                _shopOrder.CreatedById = shopOrder.CreatedById;
                _shopOrder.CreatedDate = shopOrder.CreatedDate;
                _shopOrder.ShopOrderStatusId = (long)OrderStatusEnum.OrderCreate;
                _shopOrder.ModifiedById = request.ModifiedById;
                _shopOrder.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.ShopOrder>().Update(_shopOrder);

                // ---- Get existing ShopOrderItems ----
                var existingItems = await unitOfWork
                    .Repository<ShopOrderItems>()
                    .GetAsync(x => x.ShopOrderId == shopOrder.Id && !x.IsDelete);

                // ---- Process request items (Add / Update / Soft Delete by quantity) ----
                foreach (var item in request.ShopOrderItemsList)
                {
                    // ADD NEW ITEM
                    if (item.Id == 0 && item.Quantity > 0)
                    {
                        var newItem = mapper.Map<ShopOrderItems>(item);
                        newItem.ShopOrderId = shopOrder.Id;
                        newItem.IsActive = true;
                        newItem.IsDelete = false;
                        newItem.CreatedById = request.CreatedById;
                        newItem.CreatedDate = DateTime.Now;

                        unitOfWork.Repository<ShopOrderItems>().Add(newItem);
                        continue;
                    }

                    // UPDATE OR DELETE EXISTING ITEM
                    var existingItem = existingItems.FirstOrDefault(x => x.Id == item.Id);
                    if (existingItem == null)
                        continue;

                    if (item.Quantity > 0)
                    {
                        // UPDATE
                        existingItem.Quantity = item.Quantity;
                        existingItem.Discount = item.Discount;
                        existingItem.Amount = item.Amount;
                        existingItem.ModifiedById = request.CreatedById;
                        existingItem.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        // SOFT DELETE (Quantity <= 0)
                        existingItem.IsActive = false;
                        existingItem.IsDelete = true;
                        existingItem.ModifiedById = request.CreatedById;
                        existingItem.ModifiedDate = DateTime.Now;
                    }
                }

                // ---- Soft delete items missing from request ----
                var requestItemIds = request.ShopOrderItemsList
                    .Where(x => x.Id > 0)
                    .Select(x => x.Id)
                    .ToList();

                foreach (var dbItem in existingItems)
                {
                    if (!requestItemIds.Contains(dbItem.Id))
                    {
                        dbItem.IsActive = false;
                        dbItem.IsDelete = true;
                        dbItem.ModifiedById = request.CreatedById;
                        dbItem.ModifiedDate = DateTime.Now;
                    }
                }

                // ---- Save ----
                await unitOfWork.SaveChangesAsync();
            }
            return 200;
        }
    }
}