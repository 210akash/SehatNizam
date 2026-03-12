using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrder.Handler
{
    public class GetAllShopOrderStatusHandler : IRequestHandler<GetAllShopOrderStatusQuery, List<GetStatus>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllShopOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetStatus>> Handle(GetAllShopOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.Status>().GetAllAsync();
            var order = mapper.Map<IEnumerable<GetStatus>>(entity).ToList();
            return order;
        }
    }
}
