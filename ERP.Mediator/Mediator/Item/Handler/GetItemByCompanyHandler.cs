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
    public class GetItemByCompanyHandler : IRequestHandler<GetItemByCompanyQuery, List<GetItem>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetItemByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetItem>> Handle(GetItemByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Item = await unitOfWork.Repository<Entities.Models.Item>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Item = mapper.Map<List<GetItem>>(Item);
            return _Item;
        }
    }
}
