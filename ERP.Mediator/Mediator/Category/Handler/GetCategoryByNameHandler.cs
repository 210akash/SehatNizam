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
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetCategoryByNameQuery, List<GetCategory>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCategory>> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>().GetAsync(y => y.Name == request.name);
            var _Category = mapper.Map<List<GetCategory>>(Category);
            return _Category;
        }
    }
}
