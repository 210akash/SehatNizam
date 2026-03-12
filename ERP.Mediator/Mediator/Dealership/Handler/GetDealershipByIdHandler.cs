using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dealership.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dealership.Handler
{
    public class GetDealershipByIdHandler : IRequestHandler<GetDealershipByIdQuery, GetDealership>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetDealershipByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetDealership> Handle(GetDealershipByIdQuery request, CancellationToken cancellationToken)
        {
            var dealership = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _dealership = mapper.Map<GetDealership>(dealership);
            return _dealership;
        }
    }
}
