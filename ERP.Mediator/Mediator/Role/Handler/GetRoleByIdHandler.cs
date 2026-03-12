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
    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, GetRoles>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRoleByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRoles> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(y => y.Id == new System.Guid(request.Id));
            var _role = mapper.Map<GetRoles>(role);
            return _role;
        }
    }
}
