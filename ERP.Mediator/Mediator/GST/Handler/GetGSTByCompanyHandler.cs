using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GST.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetGSTByCompanyQuery, List<GetGST>>
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

        public async Task<List<GetGST>> Handle(GetGSTByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var GST = await unitOfWork.Repository<Entities.Models.GST>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _GST = mapper.Map<List<GetGST>>(GST);
                return _GST;
            }
            else
            {
                var GST = await unitOfWork.Repository<Entities.Models.GST>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _GST = mapper.Map<List<GetGST>>(GST);
                return _GST;
            }
        }
    }
}
