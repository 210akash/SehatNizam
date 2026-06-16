using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Service.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class GetServiceByNameHandler : IRequestHandler<GetServiceByNameQuery, List<GetService>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetServiceByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetService>> Handle(GetServiceByNameQuery request, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.Repository<Entities.Models.Service>()
                .GetAsync(y => y.Name.Trim().ToLower().Contains(request.Name.Trim().ToLower()) && (request.DepartmentId == null  ||y.DepartmentId == request.DepartmentId));
            service = service.Take(10);
            var _service = mapper.Map<List<GetService>>(service);
            return _service;
        }
    }
}
