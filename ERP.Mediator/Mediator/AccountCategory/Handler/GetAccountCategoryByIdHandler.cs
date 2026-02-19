using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class GetAccountCategoryByIdHandler : IRequestHandler<GetAccountCategoryByIdQuery, GetAccountCategory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAccountCategoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetAccountCategory> Handle(GetAccountCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var AccountCategory = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _AccountCategory = mapper.Map<GetAccountCategory>(AccountCategory);
            return _AccountCategory;
        }
    }
}
