using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetIndentTypeByIdQuery, GetIndentType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetIndentType> Handle(GetIndentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _IndentType = mapper.Map<GetIndentType>(IndentType);
            return _IndentType;
        }
    }
}
