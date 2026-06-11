using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.ComponentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.ComponentType.Handler
{
    public class GetAllBloodComponentTypeHandler : IRequestHandler<GetAllBloodComponentTypeQuery, Tuple<IEnumerable<GetBloodComponentType>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllBloodComponentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetBloodComponentType>, long>> Handle(GetAllBloodComponentTypeQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.BloodComponentType, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
                && (request.Name == null || request.Name == "" || x.Name.ToLower().Contains(request.Name.ToLower().Trim()));

            Expression<Func<Entities.Models.BloodComponentType, object>>[] includes = { x => x.CreatedBy };
            Expression<Func<Entities.Models.BloodComponentType, object>> orderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.BloodComponentType>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);

            var result = mapper.Map<IEnumerable<GetBloodComponentType>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetBloodComponentType>, long>(result, entity.Item2);
        }
    }
}
