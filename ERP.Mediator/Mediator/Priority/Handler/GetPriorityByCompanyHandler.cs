using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Priority.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetPriorityByCompanyQuery, List<GetPriority>>
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

        public async Task<List<GetPriority>> Handle(GetPriorityByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _Priority = mapper.Map<List<GetPriority>>(Priority);
                return _Priority;
            }
            else
            {
                var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _Priority = mapper.Map<List<GetPriority>>(Priority);
                return _Priority;
            }
        }
    }
}
