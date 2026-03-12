using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class GetTransactionByCompanyHandler : IRequestHandler<GetTransactionByCompanyQuery, List<GetTransaction>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTransactionByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetTransaction>> Handle(GetTransactionByCompanyQuery request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetAsync(y => y.CompanyId == request.CompanyId);
            var _Transaction = mapper.Map<List<GetTransaction>>(Transaction);
            return _Transaction;
        }
    }
}
