using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class GetcommunicationModeByLocationHandler : IRequestHandler<GetStoreByLocationQuery, List<GetStore>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByLocationHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetStore>> Handle(GetStoreByLocationQuery request, CancellationToken cancellationToken)
        {
            var Store = await unitOfWork.Repository<Entities.Models.Store>().GetAsync(y => y.LocationId == request.LocationId);
            var _Store = mapper.Map<List<GetStore>>(Store);
            return _Store;
        }
    }
}
