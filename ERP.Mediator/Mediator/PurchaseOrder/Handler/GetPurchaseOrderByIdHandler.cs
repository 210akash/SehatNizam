using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQuery, GetPurchaseOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPurchaseOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetPurchaseOrder> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null, null, "PurchaseOrderDetail,PurchaseOrderDetail.PurchaseDemandDetail,PurchaseOrderDetail.PurchaseDemandDetail.Item");
            var _PurchaseOrder = mapper.Map<GetPurchaseOrder>(PurchaseOrder);
            return _PurchaseOrder;
        }
    }
}
