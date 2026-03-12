using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PaymentMode.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PaymentMode.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetPaymentModeByCompanyQuery, List<GetPaymentMode>>
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

        public async Task<List<GetPaymentMode>> Handle(GetPaymentModeByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _PaymentMode = mapper.Map<List<GetPaymentMode>>(PaymentMode);
                return _PaymentMode;
            }
            else
            {
                var PaymentMode = await unitOfWork.Repository<Entities.Models.PaymentMode>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _PaymentMode = mapper.Map<List<GetPaymentMode>>(PaymentMode);
                return _PaymentMode;
            }
        }
    }
}
