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
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetProjectByNameQuery, List<GetProject>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetProject>> Handle(GetProjectByNameQuery request, CancellationToken cancellationToken)
        {
            var Project = await unitOfWork.Repository<Entities.Models.Project>().GetAsync(y => y.Name == request.name);
            var _Project = mapper.Map<List<GetProject>>(Project);
            return _Project;
        }
    }
}
