using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Project.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetProjectByCompanyQuery, List<GetProject>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetProject>> Handle(GetProjectByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Project = await unitOfWork.Repository<Entities.Models.Project>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Project = mapper.Map<List<GetProject>>(Project);
            return _Project;
        }
    }
}
