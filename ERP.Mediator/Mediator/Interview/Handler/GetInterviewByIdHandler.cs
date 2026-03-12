using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetInterviewByIdQuery, GetInterview>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetInterview> Handle(GetInterviewByIdQuery request, CancellationToken cancellationToken)
        {
            var interview = await unitOfWork.Repository<Entities.Models.Interview>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _interview = mapper.Map<GetInterview>(interview);
            return _interview;
        }
    }
}
