using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetDispatchOrderByOrderIdHandler : IRequestHandler<GetDispatchOrderByOrderIdQuery, List<GetDispatchOrder>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDispatchOrderByOrderIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDispatchOrder>> Handle(GetDispatchOrderByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var DispatchOrder = await unitOfWork.Repository<Entities.Models.DispatchOrder>().GetAsync(x => x.OrderId == request.OrderId && x.IsActive == true, null, null,
                "Order,Order.Dealership,Dispatch,Dispatch.Vehicle,DispatchDetail,DispatchDetail.OrderItem,DispatchDetail.OrderItem.Item");
            var _DispatchOrder = mapper.Map<List<GetDispatchOrder>>(DispatchOrder);
            return _DispatchOrder;
        }
    }
}
