using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPurchaseDemandByIdHandler : IRequestHandler<GetPurchaseDemandByIdQuery, GetPurchaseDemand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPurchaseDemandByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetPurchaseDemand> Handle(GetPurchaseDemandByIdQuery request, CancellationToken cancellationToken)
        {
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _PurchaseDemand = mapper.Map<GetPurchaseDemand>(PurchaseDemand);
            return _PurchaseDemand;
        }
    }
}
