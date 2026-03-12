using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetDepartmentByCompanyQuery, List<GetDepartment>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDepartment>> Handle(GetDepartmentByCompanyQuery request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _department = mapper.Map<List<GetDepartment>>(department);
            return _department;
        }
    }
}
