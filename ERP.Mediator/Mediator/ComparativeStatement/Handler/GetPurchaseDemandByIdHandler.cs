using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class GetComparativeStatementByIdHandler : IRequestHandler<GetComparativeStatementByIdQuery, GetComparativeStatement>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetComparativeStatementByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetComparativeStatement> Handle(GetComparativeStatementByIdQuery request, CancellationToken cancellationToken)
        {
            var ComparativeStatement = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _ComparativeStatement = mapper.Map<GetComparativeStatement>(ComparativeStatement);
            return _ComparativeStatement;
        }
    }
}
