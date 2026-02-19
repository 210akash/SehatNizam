using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.UOM.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetUOMByCompanyQuery, List<GetUOM>>
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

        public async Task<List<GetUOM>> Handle(GetUOMByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _UOM = mapper.Map<List<GetUOM>>(UOM);
                return _UOM;
            }
            else
            {
                var UOM = await unitOfWork.Repository<Entities.Models.UOM>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _UOM = mapper.Map<List<GetUOM>>(UOM);
                return _UOM;
            }
        }
    }
}
