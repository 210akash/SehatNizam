using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeaveType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Handler
{
    public class GetEmployeeLeaveTypeByNameHandler : IRequestHandler<GetEmployeeLeaveTypeByNameQuery, List<GetEmployeeLeaveType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveTypeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeLeaveType>> Handle(GetEmployeeLeaveTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveType = await unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetAsync(y => y.Name == request.name);
            var _employeeLeaveType = mapper.Map<List<GetEmployeeLeaveType>>(employeeLeaveType);
            return _employeeLeaveType;
        }
    }
}
