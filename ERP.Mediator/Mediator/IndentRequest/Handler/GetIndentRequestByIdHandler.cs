using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class GetIndentRequestByIdHandler : IRequestHandler<GetIndentRequestByIdQuery, GetIndentRequest>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetIndentRequestByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetIndentRequest> Handle(GetIndentRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _IndentRequest = mapper.Map<GetIndentRequest>(IndentRequest);
            return _IndentRequest;
        }
    }
}
