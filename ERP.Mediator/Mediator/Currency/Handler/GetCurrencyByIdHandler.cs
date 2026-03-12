using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Currency.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Currency.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetCurrencyByIdQuery, GetCurrency>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetCurrency> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
        {
            var Currency = await unitOfWork.Repository<Entities.Models.Currency>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Currency = mapper.Map<GetCurrency>(Currency);
            return _Currency;
        }
    }
}
