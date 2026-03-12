using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Shop.Command;
using ERP.Mediator.Mediator.ShopDispatch.Command;
using ERP.Mediator.Mediator.ShopOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Twilio.TwiML.Voice;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class SaveShopOrderByDealershipHandler : IRequestHandler<CreateShopOrderByDealershipCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public SaveShopOrderByDealershipHandler(IMapper mapper, IUnitOfWork unitOfWork, IMediator mediator)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.mediator = mediator;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<CreateShopOrderByDealershipCommand, long>.Handle(CreateShopOrderByDealershipCommand request, CancellationToken cancellationToken)
        {
            long ShopId;
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsync(y => y.Id == request.DealershipId);
            if (dealership != null)
            {
                var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsync(y => y.TerritoryId == dealership.TerritoryId && y.Name == "Counter Sale - " + dealership.Name);

                if (shop == null)
                {

                    // Create and process the dispatch
                    var saveShopCommand = new Entities.Models.Shop
                    {
                        Id = 0, // For new dispatch, the ID is generated after insert
                        Name = "Counter Sale - " + dealership.Name,
                        OwnerName = dealership.Name,
                        Address = dealership.Address,
                        PhoneNo = dealership.PhoneNo,
                        PinLocation = dealership.PinLocation,
                        TerritoryId = dealership.TerritoryId.Value,
                        ShopTypeId = 2,
                        CreatedById = request.CreatedById,
                        CreatedDate = DateTime.Now,
                        IsVerified = true,
                        IsTagFromMob = false,
                        StatusId = 1
                    };
                    unitOfWork.Repository<Entities.Models.Shop>().Add(saveShopCommand);
                    SaveChanges();
                    request.ShopId = saveShopCommand.Id;
                }
                else
                {
                    request.ShopId = shop.Id;
                }
            }

            // Get the DatabaseFacade from the IUnitOfWork
            var databaseFacade = unitOfWork.Database();

            // Start a transaction
            using (var transaction = await databaseFacade.BeginTransactionAsync())
            {
                try
                {
                    // Map the ShopOrder entity from the request
                    var _shopOrder = mapper.Map<Entities.Models.ShopOrder>(request);
                    _shopOrder.CreatedDate = DateTime.Now;
                    _shopOrder.ShopOrderStatusId = (long)OrderStatusEnum.OrderCreate;
                    _shopOrder.ShopOrderItems = new List<ShopOrderItems>();
                    // Add the ShopOrderItems entities to the repository
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
                            _shopOrder.ShopOrderItems.Add(_shopOrderItems);
                            _shopOrder.Amount += item.Amount;
                        }
                    }
                    // Add the ShopOrder entity to the repository
                    unitOfWork.Repository<Entities.Models.ShopOrder>().Add(_shopOrder);

                    // Save the changes for ShopOrder and ShopOrderItems
                    SaveChanges();
                    // Create and process the dispatch
                    var saveShopDispatchCommand = new SaveShopDispatchCommand
                    {
                        Id = 0, // For new dispatch, the ID is generated after insert
                        ShopOrderId = _shopOrder.Id,
                        CreatedDate = DateTime.Now,
                        CreatedById = request.CreatedById,
                        Code = await GenerateDispatchCode(), // Example code generation, you can customize it
                        Remarks = "Dispatch Created for Order Counter Sale",
                        VehicleNo = "N/A", // Pass the vehicle number from the request (if applicable)
                        ShopDispatchDetail = new List<SaveShopDispatchDetailCommand>()
                    };

                    // 4. Prepare ShopDispatchDetails based on ShopOrderItems
                    foreach (var item in _shopOrder.ShopOrderItems)
                    {
                        if (item.Quantity > 0)
                        {
                            var shopDispatchDetail = new SaveShopDispatchDetailCommand
                            {
                                ShopOrderItemId = item.Id,
                                Quantity = item.Quantity, // Assuming the full quantity of the item is dispatched
                            };

                            saveShopDispatchCommand.ShopDispatchDetail.Add(shopDispatchDetail);
                        }
                    }

                    long dispatchResult = await this.mediator.Send(saveShopDispatchCommand);
                    if (dispatchResult != 200)
                    {
                        // Handle any failure cases for ShopDispatch creation
                        await transaction.RollbackAsync(); // Rollback if something goes wrong
                        return dispatchResult;
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();

                    // Return success code
                    return 200;
                }
                catch (Exception)
                {
                    // If something goes wrong, rollback the entire transaction
                    await transaction.RollbackAsync();
                    throw; // Optionally, rethrow or handle the exception
                }
            }
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