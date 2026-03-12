using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.EmployeeShift.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeShift.Handler
{
    public class GetEmployeeShiftByNameHandler : IRequestHandler<GetEmployeeShiftByNameQuery, List<GetEmployeeShift>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetEmployeeShiftByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetEmployeeShift>> Handle(GetEmployeeShiftByNameQuery request, CancellationToken cancellationToken)
        {
            var employeeShift = await unitOfWork.Repository<Entities.Models.EmployeeShift>().GetAsync(y => y.Name == request.name);
            var _employeeShift = mapper.Map<List<GetEmployeeShift>>(employeeShift);
            return _employeeShift;
        }
    }
}
