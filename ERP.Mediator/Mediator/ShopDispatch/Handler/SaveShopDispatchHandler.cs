using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ShopDispatch.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopDispatch.Handler
{
    public class SaveShopDispatchHandler : IRequestHandler<SaveShopDispatchCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IMediator mediator;

        public SaveShopDispatchHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
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

        public async Task<long> Handle(SaveShopDispatchCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.ShopDispatch>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existing == null)
                return await CreateDispatch(request);
            return 500;
            //return await UpdateDispatch(request, existing);
        }

        private async Task<long> CreateDispatch(SaveShopDispatchCommand request)
        {
            // --- Generate Dispatch Code ---
            request.Code = await GenerateDispatchCode();

            var entity = mapper.Map<Entities.Models.ShopDispatch>(request);
            entity.CreatedById = request.CreatedById;
            entity.StatusId = 3;

            // --- Fill detail meta ---
            foreach (var d in entity.ShopDispatchDetail)
            {
                d.CreatedById = request.CreatedById;
                d.CreatedDate = DateTime.Now;
            }

            // ==========================
            // PRE-SAVE VALIDATION
            // ==========================

            // Get all orderItemIds being dispatched
            var incomingOrderItemIds = entity.ShopDispatchDetail
                .Select(x => x.ShopOrderItemId)
                .ToList();

            // Load order items
            var orderItems = await unitOfWork.Repository<ShopOrderItems>()
                .GetAsync(x => incomingOrderItemIds.Contains(x.Id));

            // For each order item, check if dispatch quantity <= remaining qty
            foreach (var item in orderItems)
            {
                long orderedQty = item.Quantity;

                // Already dispatched qty (previous dispatches)
                long alreadyDispatchedQty = unitOfWork.Repository<ShopDispatchDetail>().GetAll()
                    .Where(x => x.ShopOrderItemId == item.Id && x.IsActive)
                    .Sum(x => x.Quantity);

                // Qty the user is trying to dispatch now
                long newDispatchQty = entity.ShopDispatchDetail
                    .Where(x => x.ShopOrderItemId == item.Id)
                    .Sum(x => x.Quantity);

                long remainingQty = orderedQty - alreadyDispatchedQty;

                if (newDispatchQty > remainingQty)
                {
                    throw new Exception(
                        $"You cannot dispatch more quantity than available. " +
                        $"OrderItemId: {item.Id}, Ordered: {orderedQty}, " +
                        $"Already Dispatched: {alreadyDispatchedQty}, " +
                        $"Remaining: {remainingQty}, Attempted: {newDispatchQty}"
                    );
                }
            }

            // --- Save Dispatch first ---
            unitOfWork.Repository<Entities.Models.ShopDispatch>().Add(entity);
            SaveChanges();


            // ======================================================================
            //      ORDER STATUS UPDATE BASED ON DISPATCHED QUANTITY
            // ======================================================================

            // Find all OrderIds from dispatched details
            var affectedOrderItemIds = unitOfWork.Repository<ShopDispatchDetail>().GetAllAsync().Result
                .Select(d => d.ShopOrderItemId)
                .ToList();

            // Load OrderItems for these IDs (with their ShopOrderId)
            var affectedOrderItems = await unitOfWork.Repository<ShopOrderItems>()
                .GetAsync(x => affectedOrderItemIds.Contains(x.Id));

            // Group by ShopOrderId → each represents one order to evaluate
            var ordersGrouped = affectedOrderItems.GroupBy(x => x.ShopOrderId);

            foreach (var orderGroup in ordersGrouped)
            {
                long orderId = orderGroup.Key;

                // Load ALL order items for this order (not just affected ones)
                var allOrderItems = await unitOfWork.Repository<ShopOrderItems>()
                    .GetAsync(x => x.ShopOrderId == orderId && x.IsActive);

                bool allFullyDispatched = true;

                foreach (var orderItem in allOrderItems)
                {
                    long orderedQty = orderItem.Quantity;

                    long dispatchedQty = unitOfWork.Repository<ShopDispatchDetail>().GetAllAsync().Result
                        .Where(d => d.ShopOrderItemId == orderItem.Id)
                        .Sum(d => d.Quantity);

                    if (dispatchedQty < orderedQty)
                    {
                        allFullyDispatched = false;
                        break;
                    }
                }

                // Load the shop order
                var orderHeader = await unitOfWork.Repository<Entities.Models.ShopOrder>()
                    .GetFirstAsync(x => x.Id == orderId);

                // Set correct status
                orderHeader.ShopOrderStatusId = allFullyDispatched
                    ? (long)OrderStatusEnum.OrderDispatched      // FULLY DISPATCHED
                    : (long)OrderStatusEnum.OrderPartiallyDispatched; // PARTIAL

                unitOfWork.Repository<Entities.Models.ShopOrder>().Update(orderHeader);
            }

            SaveChanges();
            return 200;
        }

        private async Task<string> GenerateDispatchCode()
        {
            var last = unitOfWork.Repository<Entities.Models.ShopDispatch>()
                .GetAllAsync().Result
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            long next = (last?.Id ?? 0) + 1;

            return next.ToString().PadLeft(7, '0');
        }
    }
}