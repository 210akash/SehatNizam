using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeaveType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveType.Handler
{
    public class GetEmployeeLeaveTypeByIdHandler : IRequestHandler<GetEmployeeLeaveTypeByIdQuery, GetEmployeeLeaveType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeLeaveType> Handle(GetEmployeeLeaveTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveType = await unitOfWork.Repository<Entities.Models.EmployeeLeaveType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeLeaveType = mapper.Map<GetEmployeeLeaveType>(employeeLeaveType);
            return _employeeLeaveType;
        }
    }
}
