using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeShift.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Handler
{
    public class GetEmployeeShiftByIdHandler : IRequestHandler<GetEmployeeShiftByIdQuery, GetEmployeeShift>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeShiftByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeShift> Handle(GetEmployeeShiftByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeShift = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeShift = mapper.Map<GetEmployeeShift>(employeeShift);
            return _employeeShift;
        }
    }
}
