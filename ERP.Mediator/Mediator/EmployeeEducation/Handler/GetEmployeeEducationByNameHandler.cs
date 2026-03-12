using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeEducation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeEducation.Handler
{
    public class GetEmployeeEducationByNameHandler : IRequestHandler<GetEmployeeEducationByNameQuery, List<GetEmployeeEducation>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeEducationByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeEducation>> Handle(GetEmployeeEducationByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeEducation = await unitOfWork.Repository<Entities.Models.EmployeeEducation>().GetAsync(y => y.Name == request.name);
            var _employeeEducation = mapper.Map<List<GetEmployeeEducation>>(employeeEducation);
            return _employeeEducation;
        }
    }
}
