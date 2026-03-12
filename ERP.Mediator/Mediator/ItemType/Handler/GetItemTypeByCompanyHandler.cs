using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class GetItemTypeByCompanyHandler : IRequestHandler<GetItemTypeByCompanyQuery, List<GetItemType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemTypeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetItemType>> Handle(GetItemTypeByCompanyQuery request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _ItemType = mapper.Map<List<GetItemType>>(ItemType);
            return _ItemType;
        }
    }
}
