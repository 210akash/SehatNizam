using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetIndentTypeByNameQuery, List<GetIndentType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetIndentType>> Handle(GetIndentTypeByNameQuery request, CancellationToken cancellationToken)
        {
            var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetAsync(y => y.Name == request.name);
            var _IndentType = mapper.Map<List<GetIndentType>>(IndentType);
            return _IndentType;
        }
    }
}
