using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class GetEmployeeLeaveGroupByNameHandler : IRequestHandler<GetEmployeeLeaveGroupByNameQuery, List<GetEmployeeLeaveGroup>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveGroupByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeLeaveGroup>> Handle(GetEmployeeLeaveGroupByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveGroup = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetAsync(y => y.Name == request.name);
            var _employeeLeaveGroup = mapper.Map<List<GetEmployeeLeaveGroup>>(employeeLeaveGroup);
            return _employeeLeaveGroup;
        }
    }
}
