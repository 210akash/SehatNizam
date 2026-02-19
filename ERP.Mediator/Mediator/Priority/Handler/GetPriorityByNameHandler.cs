using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Priority.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetPriorityByNameQuery, List<GetPriority>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPriority>> Handle(GetPriorityByNameQuery request, CancellationToken cancellationToken)
        {
            var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetAsync(y => y.Name == request.name);
            var _Priority = mapper.Map<List<GetPriority>>(Priority);
            return _Priority;
        }
    }
}
