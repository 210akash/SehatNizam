using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetPurchaseOrderByCompanyHandler : IRequestHandler<GetPurchaseOrderByCompanyQuery, List<GetPurchaseOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPurchaseOrderByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPurchaseOrder>> Handle(GetPurchaseOrderByCompanyQuery request, CancellationToken cancellationToken)
        {
            var PurchaseOrder = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetAsync();
            var _PurchaseOrder = mapper.Map<List<GetPurchaseOrder>>(PurchaseOrder);
            return _PurchaseOrder;
        }
    }
}
