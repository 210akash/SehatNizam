using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetIndentTypeByCompanyQuery, List<GetIndentType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetcommunicationModeByCompanyHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetIndentType>> Handle(GetIndentTypeByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _IndentType = mapper.Map<List<GetIndentType>>(IndentType);
                return _IndentType;
            }
            else
            {
                var IndentType = await unitOfWork.Repository<Entities.Models.IndentType>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _IndentType = mapper.Map<List<GetIndentType>>(IndentType);
                return _IndentType;
            }
        }
    }
}
