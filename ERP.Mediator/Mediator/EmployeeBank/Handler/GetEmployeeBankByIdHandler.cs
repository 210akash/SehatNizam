using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeBank.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Handler
{
    public class GetEmployeeBankByIdHandler : IRequestHandler<GetEmployeeBankByIdQuery, GetEmployeeBank>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeBankByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetEmployeeBank> Handle(GetEmployeeBankByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeBank = await unitOfWork.Repository<Entities.Models.EmployeeBank>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _employeeBank = mapper.Map<GetEmployeeBank>(employeeBank);
            return _employeeBank;
        }
    }
}
