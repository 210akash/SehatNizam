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
    public class GetAllRadiologyTypesHandler : IRequestHandler<GetAllRadiologyTypesQuery, Tuple<IEnumerable<GetRadiologyType>, long>>
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

        public async Task<Tuple<IEnumerable<GetRadiologyType>, long>> Handle(GetAllRadiologyTypesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<ERP.Entities.Models.RadiologyType, bool>> predicate = x =>
                x.IsActive == true &&
                x.IsDelete == false;

            if (request.ServiceId.HasValue && request.ServiceId.Value > 0)
            {
                predicate = x => x.IsActive == true &&
                    x.IsDelete == false &&
                    x.CompanyId == this.sessionProvider.Session.CompanyId &&
                    x.ServiceId == request.ServiceId.Value;
            }

            Expression<Func<Entities.Models.RadiologyType, object>> OrderBy = null;
            Expression<Func<Entities.Models.RadiologyType, object>> OrderByDesc = x => x.Id;
            Expression<Func<ERP.Entities.Models.RadiologyType, object>>[] includes = {
                x => x.Service
            };

            var entity = unitOfWork.Repository<Entities.Models.RadiologyType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var radiologyType = mapper.Map<IEnumerable<GetRadiologyType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRadiologyType>, long>(radiologyType, entity.Item2);
        }
    }
}
