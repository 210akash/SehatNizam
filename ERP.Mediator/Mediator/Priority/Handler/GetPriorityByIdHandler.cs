using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Priority.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetPriorityByIdQuery, GetPriority>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetPriority> Handle(GetPriorityByIdQuery request, CancellationToken cancellationToken)
        {
            var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Priority = mapper.Map<GetPriority>(Priority);
            return _Priority;
        }
    }
}
