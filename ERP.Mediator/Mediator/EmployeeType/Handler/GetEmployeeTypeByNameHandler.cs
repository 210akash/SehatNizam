using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeType.Handler
{
    public class GetEmployeeTypeByNameHandler : IRequestHandler<GetEmployeeTypeByNameQuery, List<GetEmployeeType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeTypeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeType>> Handle(GetEmployeeTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeType = await unitOfWork.Repository<Entities.Models.EmployeeType>().GetAsync(y => y.Name == request.name);
            var _employeeType = mapper.Map<List<GetEmployeeType>>(employeeType);
            return _employeeType;
        }
    }
}
