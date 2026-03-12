using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PrimaryOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PrimaryOrder.Handler
{
    public class GetAllOrderStatusHandler : IRequestHandler<GetAllOrderStatusQuery, List<GetStatus>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetAllOrderStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetStatus>> Handle(GetAllOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.Status>().GetAllAsync();
            var order = mapper.Map<IEnumerable<GetStatus>>(entity).ToList();
            return order;
        }
    }
}
