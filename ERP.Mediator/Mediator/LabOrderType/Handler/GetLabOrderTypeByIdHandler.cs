using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.LabOrderType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrderType.Handler
{
    public class GetLabOrderTypeByIdHandler : IRequestHandler<GetLabOrderTypeByIdQuery, GetLabOrderType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetLabOrderTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetLabOrderType> Handle(GetLabOrderTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.LabOrderType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id && x.IsActive == true);
            return mapper.Map<GetLabOrderType>(entity);
        }
    }
}
