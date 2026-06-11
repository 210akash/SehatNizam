using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Issue.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Handler
{
    public class GetAllBloodIssueHandler : IRequestHandler<GetAllBloodIssueQuery, Tuple<IEnumerable<GetBloodIssue>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllBloodIssueHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetBloodIssue>, long>> Handle(GetAllBloodIssueQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.BloodIssue, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
                && (!request.BloodRequestId.HasValue || request.BloodRequestId == 0 || x.BloodRequestId == request.BloodRequestId)
                && (request.RequestCode == null || request.RequestCode == "" || (x.BloodRequest.Code ?? "").ToLower().Contains(request.RequestCode.ToLower().Trim()))
                && (request.IssuedTo == null || request.IssuedTo == "" || (x.IssuedTo ?? "").ToLower().Contains(request.IssuedTo.ToLower().Trim()));

            Expression<Func<Entities.Models.BloodIssue, object>>[] includes =
            {
                x => x.CreatedBy,
                x => x.BloodRequest,
                x => x.BloodRequest.BloodGroupMaster,
                x => x.BloodRequest.BloodComponentType,
                x => x.BloodUnit,
                x => x.BloodUnit.BloodGroupMaster,
                x => x.BloodUnit.BloodComponentType,
                x => x.BloodUnit.BloodFridge,
                x => x.BloodUnit.BloodRack,
                x => x.BloodCrossMatch
            };
            Expression<Func<Entities.Models.BloodIssue, object>> orderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);

            var result = mapper.Map<IEnumerable<GetBloodIssue>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetBloodIssue>, long>(result, entity.Item2);
        }
    }
}
