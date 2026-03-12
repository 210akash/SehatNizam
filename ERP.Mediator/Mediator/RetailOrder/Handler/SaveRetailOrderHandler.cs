using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RetailOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrder.Handler
{
    public class SaveRetailOrderHandler : IRequestHandler<CreateRetailOrderCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRetailOrderHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<CreateRetailOrderCommand, long>.Handle(CreateRetailOrderCommand request, CancellationToken cancellationToken)
        {
            var RetailOrder = await unitOfWork.Repository<Entities.Models.RetailOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (RetailOrder == null)
            {
                var _RetailOrder = mapper.Map<Entities.Models.RetailOrder>(request);
                _RetailOrder.ShopId = sessionProvider.Session.RetailUserShopId;
                _RetailOrder.CreatedById = sessionProvider.Session.LoggedInUserId;
                _RetailOrder.CreatedDate = DateTime.Now;
                _RetailOrder.RetailOrderStatusId = (long)OrderStatusEnum.OrderCreate;

                unitOfWork.Repository<Entities.Models.RetailOrder>().Add(_RetailOrder);
                SaveChanges();

                foreach (var item in request.RetailOrderItemsList)
                {
                    if (item.Quantity > 0)
                    {
                        var _RetailOrderItems = mapper.Map<Entities.Models.RetailOrderItems>(item);
                        _RetailOrderItems.IsActive = true;
                        _RetailOrderItems.IsDelete = false;
                        _RetailOrderItems.RetailOrderId = _RetailOrder.Id;
                        _RetailOrderItems.CreatedById = sessionProvider.Session.LoggedInUserId;
                        _RetailOrderItems.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.RetailOrderItems>().Add(_RetailOrderItems);
                    }
                }

                RetailOrderProcess process = new RetailOrderProcess();
                process.RetailOrderId = _RetailOrder.Id;
                process.FromStatusId = null;
                process.ToStatusId = _RetailOrder.RetailOrderStatusId;
                process.Comments = "New Order Created!";
                process.CreatedById = sessionProvider.Session.LoggedInUserId;
                process.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.RetailOrderProcess>().Add(process);
                SaveChanges();
            }
            else
            {
                var _RetailOrder = mapper.Map<Entities.Models.RetailOrder>(request);
                _RetailOrder.CreatedById = RetailOrder.CreatedById;
                _RetailOrder.CreatedDate = RetailOrder.CreatedDate;
                _RetailOrder.RetailOrderStatusId = RetailOrder.RetailOrderStatusId;
                _RetailOrder.ShopId = RetailOrder.ShopId;
                _RetailOrder.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _RetailOrder.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.RetailOrder>().Update(_RetailOrder);

                foreach (var item in request.RetailOrderItemsList)
                {
                    var _RetailOrderItems = mapper.Map<Entities.Models.RetailOrderItems>(item);
                    _RetailOrderItems.RetailOrderId = _RetailOrder.Id;
                    _RetailOrderItems.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _RetailOrderItems.CreatedDate = DateTime.Now;
                    _RetailOrderItems.IsActive = true;
                    _RetailOrderItems.IsDelete = false;
                    unitOfWork.Repository<Entities.Models.RetailOrderItems>().Update(_RetailOrderItems);
                }

                var process = await unitOfWork.Repository<Entities.Models.RetailOrderProcess>().GetFirstAsNoTrackingAsync(x => x.RetailOrderId == request.Id);
                process.ModifiedById = sessionProvider.Session.LoggedInUserId;
                process.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.RetailOrderProcess>().Update(process);

                SaveChanges();
            }
            return 200;
        }
    }
}