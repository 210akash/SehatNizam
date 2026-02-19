using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetAccountCategoryByCompanyQuery, List<GetAccountCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetAccountCategory>> Handle(GetAccountCategoryByCompanyQuery request, CancellationToken cancellationToken)
        {
            var AccountCategory = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _AccountCategory = mapper.Map<List<GetAccountCategory>>(AccountCategory);
            return _AccountCategory;
        }
    }
}
