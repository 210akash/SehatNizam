using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeaveGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeaveGroup.Handler
{
    public class GetEmployeeLeaveGroupByIdHandler : IRequestHandler<GetEmployeeLeaveGroupByIdQuery, GetEmployeeLeaveGroup>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveGroupByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeLeaveGroup> Handle(GetEmployeeLeaveGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeLeaveGroup = await unitOfWork.Repository<Entities.Models.EmployeeLeaveGroup>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeLeaveGroup = mapper.Map<GetEmployeeLeaveGroup>(employeeLeaveGroup);
            return _employeeLeaveGroup;
        }
    }
}
