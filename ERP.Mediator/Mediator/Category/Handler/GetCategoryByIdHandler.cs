using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Category.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetCategoryByIdQuery, GetCategory>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetCategory> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Category = mapper.Map<GetCategory>(Category);
            return _Category;
        }
    }
}
