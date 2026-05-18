using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.ServiceType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ServiceTypes.Handler
{
    public class GetAllServiceTypesHandler : IRequestHandler<GetAllServiceTypesQuery, Tuple<IEnumerable<GetServiceType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllServiceTypesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetServiceType>, long>> Handle(GetAllServiceTypesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.ServiceType, bool>> predicate = x => x.IsActive == true
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()));

            Expression<Func<Entities.Models.ServiceType, object>> OrderBy = null;
            Expression<Func<Entities.Models.ServiceType, object>> OrderByDescending = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.ServiceType>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDescending, null, null);
            var ServiceTypes = mapper.Map<IEnumerable<GetServiceType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetServiceType>, long>(ServiceTypes, entity.Item2);
        }
    }
}
