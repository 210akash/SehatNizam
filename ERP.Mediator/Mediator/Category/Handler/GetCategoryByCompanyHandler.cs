using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Category.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetCategoryByCompanyQuery, List<GetCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCategory>> Handle(GetCategoryByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Category = mapper.Map<List<GetCategory>>(Category);
            return _Category;
        }
    }
}
