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
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetLocationByCompanyQuery, List<GetLocation>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetLocation>> Handle(GetLocationByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Location = await unitOfWork.Repository<Entities.Models.Location>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Location = mapper.Map<List<GetLocation>>(Location);
            return _Location;
        }
    }
}
