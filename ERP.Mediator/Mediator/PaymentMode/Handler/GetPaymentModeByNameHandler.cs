using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetPaymentModeByNameQuery, List<GetPaymentMode>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetPaymentMode>> Handle(GetPaymentModeByNameQuery request, CancellationToken cancellationToken)
        {
            var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetAsync(y => y.Name == request.name);
            var _PaymentMode = mapper.Map<List<GetPaymentMode>>(PaymentMode);
            return _PaymentMode;
        }
    }
}
