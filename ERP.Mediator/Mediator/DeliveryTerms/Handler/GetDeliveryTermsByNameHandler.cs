using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.DeliveryTerms.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetDeliveryTermsByNameQuery, List<GetDeliveryTerms>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDeliveryTerms>> Handle(GetDeliveryTermsByNameQuery request, CancellationToken cancellationToken)
        {
            var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetAsync(y => y.Name == request.name);
            var _DeliveryTerms = mapper.Map<List<GetDeliveryTerms>>(DeliveryTerms);
            return _DeliveryTerms;
        }
    }
}
