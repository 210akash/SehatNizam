using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.DeliveryTerms.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetDeliveryTermsByIdQuery, GetDeliveryTerms>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetDeliveryTerms> Handle(GetDeliveryTermsByIdQuery request, CancellationToken cancellationToken)
        {
            var DeliveryTerms = await unitOfWork.Repository<Entities.Models.DeliveryTerms>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _DeliveryTerms = mapper.Map<GetDeliveryTerms>(DeliveryTerms);
            return _DeliveryTerms;
        }
    }
}
