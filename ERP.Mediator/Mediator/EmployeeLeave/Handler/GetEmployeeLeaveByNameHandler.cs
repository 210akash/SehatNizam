using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class GetEmployeeLeaveByNameHandler : IRequestHandler<GetEmployeeLeaveByNameQuery, List<GetEmployeeLeave>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeLeave>> Handle(GetEmployeeLeaveByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeLeave = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetAsync(y => y.Comments == request.name);
            var _employeeLeave = mapper.Map<List<GetEmployeeLeave>>(employeeLeave);
            return _employeeLeave;
        }
    }
}
