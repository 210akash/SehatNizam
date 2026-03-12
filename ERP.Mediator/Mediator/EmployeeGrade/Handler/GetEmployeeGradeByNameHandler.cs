using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeGrade.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeGrade.Handler
{
    public class GetEmployeeGradeByNameHandler : IRequestHandler<GetEmployeeGradeByNameQuery, List<GetEmployeeGrade>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeGradeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeGrade>> Handle(GetEmployeeGradeByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeGrade = await unitOfWork.Repository<Entities.Models.EmployeeGrade>().GetAsync(y => y.Name == request.name);
            var _employeeGrade = mapper.Map<List<GetEmployeeGrade>>(employeeGrade);
            return _employeeGrade;
        }
    }
}
