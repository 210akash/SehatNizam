using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeDesignation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDesignation.Handler
{
    public class GetEmployeeDesignationByNameHandler : IRequestHandler<GetEmployeeDesignationByNameQuery, List<GetEmployeeDesignation>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeDesignationByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeDesignation>> Handle(GetEmployeeDesignationByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeDesignation = await unitOfWork.Repository<Entities.Models.EmployeeDesignation>().GetAsync(y => y.Name == request.name);
            var _employeeDesignation = mapper.Map<List<GetEmployeeDesignation>>(employeeDesignation);
            return _employeeDesignation;
        }
    }
}
