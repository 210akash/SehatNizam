using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class GetItemTypeByIdHandler : IRequestHandler<GetItemTypeByIdQuery, GetItemType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetItemType> Handle(GetItemTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _ItemType = mapper.Map<GetItemType>(ItemType);
            return _ItemType;
        }
    }
}
