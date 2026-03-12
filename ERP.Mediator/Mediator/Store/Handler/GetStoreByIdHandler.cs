using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetStoreByIdQuery, GetStore>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetStore> Handle(GetStoreByIdQuery request, CancellationToken cancellationToken)
        {
            var Store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Store = mapper.Map<GetStore>(Store);
            return _Store;
        }
    }
}
