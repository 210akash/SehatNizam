using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Query;
using ERP.Mediator.Mediator.Role.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class GetAllRolesByDepartmentHandler : IRequestHandler<GetAllRolesByDepartmentQuery, List<GetRoles>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllRolesByDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoles>> Handle(GetAllRolesByDepartmentQuery request, CancellationToken cancellationToken)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().GetAsync(y => y.IsActive == true && y.IsDelete == false && y.DepartmentId == request.departmentId);
            var _role = mapper.Map<List<GetRoles>>(role);
            return _role;
        }
    }
}
