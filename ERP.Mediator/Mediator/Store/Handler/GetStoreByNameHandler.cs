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
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetStoreByNameQuery, List<GetStore>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetStore>> Handle(GetStoreByNameQuery request, CancellationToken cancellationToken)
        {
            var Store = await unitOfWork.Repository<Entities.Models.Store>().GetAsync(y => y.Name == request.name);
            var _Store = mapper.Map<List<GetStore>>(Store);
            return _Store;
        }
    }
}
