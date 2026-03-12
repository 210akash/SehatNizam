using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeBank.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeBank.Handler
{
    public class GetEmployeeBankByNameHandler : IRequestHandler<GetEmployeeBankByNameQuery, List<GetEmployeeBank>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeBankByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeBank>> Handle(GetEmployeeBankByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeBank = await unitOfWork.Repository<Entities.Models.EmployeeBank>().GetAsync(y => y.BankName == request.name);
            var _employeeBank = mapper.Map<List<GetEmployeeBank>>(employeeBank);
            return _employeeBank;
        }
    }
}
