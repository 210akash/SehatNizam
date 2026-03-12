using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPurchaseDemandByCompanyHandler : IRequestHandler<GetPurchaseDemandByCompanyQuery, List<GetPurchaseDemand>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPurchaseDemandByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPurchaseDemand>> Handle(GetPurchaseDemandByCompanyQuery request, CancellationToken cancellationToken)
        {
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetAsync();
            var _PurchaseDemand = mapper.Map<List<GetPurchaseDemand>>(PurchaseDemand);
            return _PurchaseDemand;
        }
    }
}
