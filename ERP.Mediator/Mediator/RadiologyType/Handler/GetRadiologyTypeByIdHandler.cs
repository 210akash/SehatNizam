using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.RadiologyType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Handler
{
    public class GetRadiologyTypeByIdHandler : IRequestHandler<GetRadiologyTypeByIdQuery, GetRadiologyType>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRadiologyTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRadiologyType> Handle(GetRadiologyTypeByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<System.Func<ERP.Entities.Models.RadiologyType, object>>[] includes = {
                x => x.Service
            };

            var radiologyType = await unitOfWork.Repository<ERP.Entities.Models.RadiologyType>().GetByIdAsync(request.Id, includes);
            if (radiologyType == null || radiologyType.IsDelete)
            {
                return null;
            }

            var result = mapper.Map<GetRadiologyType>(radiologyType);

            if (radiologyType.Service != null)
            {
                result.ServiceName = radiologyType.Service.Name;
            }

            return result;
        }
    }
}
