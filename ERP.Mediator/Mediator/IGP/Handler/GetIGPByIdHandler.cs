using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetIGPByIdHandler : IRequestHandler<GetIGPByIdQuery, GetIGP>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetIGPByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetIGP> Handle(GetIGPByIdQuery request, CancellationToken cancellationToken)
        {
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _IGP = mapper.Map<GetIGP>(IGP);
            return _IGP;
        }
    }
}
