using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Location.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Location.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetLocationByNameQuery, List<GetLocation>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetLocation>> Handle(GetLocationByNameQuery request, CancellationToken cancellationToken)
        {
            var Location = await unitOfWork.Repository<Entities.Models.Location>().GetAsync(y => y.Name == request.name);
            var _Location = mapper.Map<List<GetLocation>>(Location);
            return _Location;
        }
    }
}
