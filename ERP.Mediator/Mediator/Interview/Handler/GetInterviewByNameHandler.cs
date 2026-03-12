using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetInterviewByNameQuery, List<GetInterview>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetInterview>> Handle(GetInterviewByNameQuery request, CancellationToken cancellationToken)
        {
            var interview = await unitOfWork.Repository<Entities.Models.Interview>().GetAsync(y => y.Name == request.name);
            var _interview = mapper.Map<List<GetInterview>>(interview);
            return _interview;
        }
    }
}
