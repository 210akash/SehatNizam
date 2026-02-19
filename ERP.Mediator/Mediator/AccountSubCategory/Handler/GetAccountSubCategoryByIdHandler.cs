using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetAccountSubCategoryByIdQuery, GetAccountSubCategory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetAccountSubCategory> Handle(GetAccountSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _AccountSubCategory = mapper.Map<GetAccountSubCategory>(AccountSubCategory);
            return _AccountSubCategory;
        }
    }
}
