using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class GetRadiologyOrderByIdHandler : IRequestHandler<GetRadiologyOrderByIdQuery, GetRadiologyOrder>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRadiologyOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRadiologyOrder> Handle(GetRadiologyOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<System.Func<ERP.Entities.Models.RadiologyOrder, object>>[] includes = {
                x => x.RadiologyType
            };

            var radiologyOrder = await unitOfWork.Repository<ERP.Entities.Models.RadiologyOrder>().GetByIdAsync(request.Id, includes);
            if (radiologyOrder == null || radiologyOrder.IsDelete)
            {
                return null;
            }

            var result = mapper.Map<GetRadiologyOrder>(radiologyOrder);

            if (radiologyOrder.RadiologyType != null)
            {
                result.RadiologyTypeName = radiologyOrder.RadiologyType.Name;
            }

            return result;
        }
    }
}
