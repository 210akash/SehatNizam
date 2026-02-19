using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetCurrencyByNameQuery, List<GetCurrency>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCurrency>> Handle(GetCurrencyByNameQuery request, CancellationToken cancellationToken)
        {
            var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetAsync(y => y.Name == request.name);
            var _Currency = mapper.Map<List<GetCurrency>>(Currency);
            return _Currency;
        }
    }
}
