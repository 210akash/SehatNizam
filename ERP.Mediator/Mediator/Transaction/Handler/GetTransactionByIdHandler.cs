using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, GetTransaction>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTransactionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetTransaction> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Transaction = mapper.Map<GetTransaction>(Transaction);
            return _Transaction;
        }
    }
}
