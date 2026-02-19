using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetPaymentModeByIdQuery, GetPaymentMode>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetPaymentMode> Handle(GetPaymentModeByIdQuery request, CancellationToken cancellationToken)
        {
            var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _PaymentMode = mapper.Map<GetPaymentMode>(PaymentMode);
            return _PaymentMode;
        }
    }
}
