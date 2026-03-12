using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Rack.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class GetRackByIdHandler : IRequestHandler<GetRackByIdQuery, GetRack>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRackByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRack> Handle(GetRackByIdQuery request, CancellationToken cancellationToken)
        {
            var Rack = await unitOfWork.Repository<Entities.Models.Rack>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Rack = mapper.Map<GetRack>(Rack);
            return _Rack;
        }
    }
}
