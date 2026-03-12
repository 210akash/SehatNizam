using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Route.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class GetRouteByNameHandler : IRequestHandler<GetRouteByNameQuery, List<GetRoute>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRouteByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoute>> Handle(GetRouteByNameQuery request, CancellationToken cancellationToken)
        {
            var route = await unitOfWork.Repository<Entities.Models.Route>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _route = mapper.Map<List<GetRoute>>(route);
            return _route;
        }
    }
}
