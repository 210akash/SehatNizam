using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Role.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Handler
{
    public class GetRoleByNameHandler : IRequestHandler<GetRoleByNameQuery, List<GetRoles>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRoleByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetRoles>> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().GetAsync(y => y.Name.ToLower().Contains(request.name));
            var _role = mapper.Map<List<GetRoles>>(role);
            return _role;
        }
    }
}
