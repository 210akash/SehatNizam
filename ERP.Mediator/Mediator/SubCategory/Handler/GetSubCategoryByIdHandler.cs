using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetSubCategoryByIdQuery, GetSubCategory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetSubCategory> Handle(GetSubCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _SubCategory = mapper.Map<GetSubCategory>(SubCategory);
            return _SubCategory;
        }
    }
}
