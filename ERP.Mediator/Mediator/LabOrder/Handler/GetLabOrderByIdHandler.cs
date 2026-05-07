using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class GetLabOrderByIdHandler : IRequestHandler<GetLabOrderByIdQuery, GetLabOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GetLabOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<GetLabOrder> Handle(GetLabOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id && x.IsActive == true);
            return mapper.Map<GetLabOrder>(entity);
        }
    }
}
