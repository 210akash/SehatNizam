using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class GetComparativeStatementByCompanyHandler : IRequestHandler<GetComparativeStatementByCompanyQuery, List<GetComparativeStatement>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetComparativeStatementByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetComparativeStatement>> Handle(GetComparativeStatementByCompanyQuery request, CancellationToken cancellationToken)
        {
            var ComparativeStatement = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetAsync();
            var _ComparativeStatement = mapper.Map<List<GetComparativeStatement>>(ComparativeStatement);
            return _ComparativeStatement;
        }
    }
}
