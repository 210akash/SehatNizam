using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.DeliveryTerms.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Handler
{
    public class GetcommunicationModeByCompanyHandler : IRequestHandler<GetDeliveryTermsByCompanyQuery, List<GetDeliveryTerms>>
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

        public async Task<List<GetDeliveryTerms>> Handle(GetDeliveryTermsByCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId != 0)
            {
                var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetAsync(y => y.CompanyId == request.CompanyId);
                var _DeliveryTerms = mapper.Map<List<GetDeliveryTerms>>(DeliveryTerms);
                return _DeliveryTerms;
            }
            else
            {
                var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetAsync(y => y.CompanyId == sessionProvider.Session.CompanyId);
                var _DeliveryTerms = mapper.Map<List<GetDeliveryTerms>>(DeliveryTerms);
                return _DeliveryTerms;
            }
        }
    }
}
