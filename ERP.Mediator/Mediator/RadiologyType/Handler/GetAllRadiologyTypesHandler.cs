using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyType.Handler
{
    public class GetAllRadiologyTypesHandler : IRequestHandler<GetAllRadiologyTypesQuery, IEnumerable<GetRadiologyType>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRadiologyTypesHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetRadiologyType>> Handle(GetAllRadiologyTypesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ERP.Entities.Models.RadiologyType, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false &&
                x.CompanyId == this.sessionProvider.Session.CompanyId;

            if (request.ServiceId.HasValue && request.ServiceId.Value > 0)
            {
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.CompanyId == this.sessionProvider.Session.CompanyId &&
                    x.ServiceId == request.ServiceId.Value;
            }

            Expression<Func<ERP.Entities.Models.RadiologyType, object>>[] includes = {
                x => x.Service
            };

            var radiologyTypes = await unitOfWork.Repository<ERP.Entities.Models.RadiologyType>().GetWhereAsync(predicate, null, includes);
            var result = mapper.Map<IEnumerable<GetRadiologyType>>(radiologyTypes.ToList());

            foreach (var item in result)
            {
                var entity = radiologyTypes.FirstOrDefault(x => x.Id == item.Id);
                if (entity != null && entity.Service != null)
                {
                    item.ServiceName = entity.Service.Name;
                }
            }

            return result.OrderBy(x => x.Name);
        }
    }
}
