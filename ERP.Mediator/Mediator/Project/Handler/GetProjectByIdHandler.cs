using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Project.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetProjectByIdQuery, GetProject>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetProject> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var Project = await unitOfWork.Repository<Entities.Models.Project>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Project = mapper.Map<GetProject>(Project);
            return _Project;
        }
    }
}
