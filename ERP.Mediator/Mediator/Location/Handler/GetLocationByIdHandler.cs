using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Location.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetLocationByIdQuery, GetLocation>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetLocation> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var Location = await unitOfWork.Repository<Entities.Models.Location>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Location = mapper.Map<GetLocation>(Location);
            return _Location;
        }
    }
}
