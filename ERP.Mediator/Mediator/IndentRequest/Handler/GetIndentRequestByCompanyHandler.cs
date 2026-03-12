using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class GetIndentRequestByCompanyHandler : IRequestHandler<GetIndentRequestByCompanyQuery, List<GetIndentRequest>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetIndentRequestByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetIndentRequest>> Handle(GetIndentRequestByCompanyQuery request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetAsync(y => y.Department.CompanyId == request.CompanyId);
            var _IndentRequest = mapper.Map<List<GetIndentRequest>>(IndentRequest);
            return _IndentRequest;
        }
    }
}
