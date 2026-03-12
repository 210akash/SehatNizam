using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Rack.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class GetRackByNameHandler : IRequestHandler<GetRackByNameQuery, List<GetRack>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRackByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRack>> Handle(GetRackByNameQuery request, CancellationToken cancellationToken)
        {
            var Rack = await unitOfWork.Repository<Entities.Models.Rack>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _Rack = mapper.Map<List<GetRack>>(Rack);
            return _Rack;
        }
    }
}
