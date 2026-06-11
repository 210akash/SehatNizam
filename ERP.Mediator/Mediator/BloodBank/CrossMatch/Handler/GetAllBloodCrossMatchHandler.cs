using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.CrossMatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Handler
{
    public class GetAllBloodCrossMatchHandler : IRequestHandler<GetAllBloodCrossMatchQuery, Tuple<IEnumerable<GetBloodCrossMatch>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllBloodCrossMatchHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetBloodCrossMatch>, long>> Handle(GetAllBloodCrossMatchQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.BloodCrossMatch, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
                && (!request.BloodRequestId.HasValue || request.BloodRequestId == 0 || x.BloodRequestId == request.BloodRequestId);

            Expression<Func<Entities.Models.BloodCrossMatch, object>>[] includes =
            {
                x => x.CreatedBy,
                x => x.BloodRequest,
                x => x.BloodRequest.BloodGroupMaster,
                x => x.BloodRequest.BloodComponentType,
                x => x.BloodUnit,
                x => x.BloodUnit.BloodGroupMaster,
                x => x.BloodUnit.BloodComponentType,
                x => x.BloodUnit.BloodFridge,
                x => x.BloodUnit.BloodRack
            };
            Expression<Func<Entities.Models.BloodCrossMatch, object>> orderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);

            var result = mapper.Map<IEnumerable<GetBloodCrossMatch>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetBloodCrossMatch>, long>(result, entity.Item2);
        }
    }
}
