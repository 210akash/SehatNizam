using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Handler
{
    public class GetEmployeeTypeByIdHandler : IRequestHandler<GetEmployeeTypeByIdQuery, GetEmployeeType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeType> Handle(GetEmployeeTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeType = await unitOfWork.Repository<Entities.Models.EmployeeType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeType = mapper.Map<GetEmployeeType>(employeeType);
            return _employeeType;
        }
    }
}
