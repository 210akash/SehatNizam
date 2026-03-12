using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class GetCostSheetByItemHandler : IRequestHandler<GetCostSheetByItemQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCostSheetByItemHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDropDown>> Handle(GetCostSheetByItemQuery request, CancellationToken cancellationToken)
        {
            var costsheet = await unitOfWork.Repository<Entities.Models.CostSheet>().GetAsync(y => y.ItemId == request.ItemId);
            var _costsheet = mapper.Map<List<GetDropDown>>(costsheet);
            return _costsheet;
        }
    }
}
