using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, GetItem>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetItem> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var Item = await unitOfWork.Repository<Entities.Models.Item>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Item = mapper.Map<GetItem>(Item);
            return _Item;
        }
    }
}
