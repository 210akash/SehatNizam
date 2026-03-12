using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetCurrencyByCompanyQuery, List<GetCurrency>>
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

        public async Task<List<GetCurrency>> Handle(GetCurrencyByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _Currency = mapper.Map<List<GetCurrency>>(Currency);
                return _Currency;
            }
            else
            {
                var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _Currency = mapper.Map<List<GetCurrency>>(Currency);
                return _Currency;
            }
        }
    }
}
