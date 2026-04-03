using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class GetClinicalDepartmentHandler : IRequestHandler<GetClinicalDepartmentQuery, List<GetDepartment>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;


        public GetClinicalDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetDepartment>> Handle(GetClinicalDepartmentQuery request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId
             && y.Clinical);
            var _department = mapper.Map<List<GetDepartment>>(department);
            return _department;
        }
    }
}
