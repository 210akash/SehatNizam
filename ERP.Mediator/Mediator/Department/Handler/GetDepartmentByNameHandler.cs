using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetDepartmentByNameQuery, List<GetDepartment>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDepartment>> Handle(GetDepartmentByNameQuery request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetAsync(y => y.Name == request.name);
            var _department = mapper.Map<List<GetDepartment>>(department);
            return _department;
        }
    }
}
