using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetIssuanceByIdHandler : IRequestHandler<GetIssuanceByIdQuery, GetIssuance>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetIssuanceByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetIssuance> Handle(GetIssuanceByIdQuery request, CancellationToken cancellationToken)
        {
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id, null, null, "IssuanceDetail,IssuanceDetail.PurchaseDemandDetail,IssuanceDetail.PurchaseDemandDetail.Item");
            var _Issuance = mapper.Map<GetIssuance>(Issuance);
            return _Issuance;
        }
    }
}
