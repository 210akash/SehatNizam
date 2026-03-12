using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetInspectionByIdHandler : IRequestHandler<GetInspectionByIdQuery, GetInspection>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetInspectionByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetInspection> Handle(GetInspectionByIdQuery request, CancellationToken cancellationToken)
        {
            var Inspection = await unitOfWork.Repository<Entities.Models.Inspection>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _Inspection = mapper.Map<GetInspection>(Inspection);
            return _Inspection;
        }
    }
}
