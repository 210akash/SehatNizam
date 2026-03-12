using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeLeave.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Handler
{
    public class GetEmployeeLeaveByIdHandler : IRequestHandler<GetEmployeeLeaveByIdQuery, GetEmployeeLeave>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeLeaveByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeLeave> Handle(GetEmployeeLeaveByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeLeave = await unitOfWork.Repository<Entities.Models.EmployeeLeave>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeLeave = mapper.Map<GetEmployeeLeave>(employeeLeave);
            return _employeeLeave;
        }
    }
}
