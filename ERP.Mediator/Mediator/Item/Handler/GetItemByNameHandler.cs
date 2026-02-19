using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetItemByNameHandler : IRequestHandler<GetItemByNameQuery, List<GetItem>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetItem>> Handle(GetItemByNameQuery request, CancellationToken cancellationToken)
        {
            var Item = await unitOfWork.Repository<Entities.Models.Item>().GetAsync(y => y.Name.ToLower().Contains(request.name.ToLower()) || y.Code.ToLower().Contains(request.name.ToLower()),null,null, "UOM",0,10);
            var _Item = mapper.Map<List<GetItem>>(Item);
            return _Item;
        }
    }
}
