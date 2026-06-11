using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Issue.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Issue.Handler
{
    public class GetBloodIssueWorklistHandler : IRequestHandler<GetBloodIssueWorklistQuery, Tuple<IEnumerable<GetBloodIssueWorklist>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBloodIssueWorklistHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetBloodIssueWorklist>, long>> Handle(GetBloodIssueWorklistQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.BloodRequest, bool>> predicate = x => x.IsActive == true && x.IsDelete == false
                && x.Status == (int)BloodRequestStatus.CrossMatched
                && (request.RequestCode == null || request.RequestCode == "" || (x.Code ?? "").ToLower().Contains(request.RequestCode.ToLower().Trim()));

            Expression<Func<Entities.Models.BloodRequest, object>>[] includes =
            {
                x => x.BloodGroupMaster,
                x => x.BloodComponentType,
                x => x.Admission
            };

            Expression<Func<Entities.Models.BloodRequest, object>> orderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, orderByDesc, null, includes);

            var requests = entity.Item1.ToList();
            var requestIds = requests.Select(x => x.Id).ToList();

            if (requestIds.Count == 0)
            {
                return new Tuple<IEnumerable<GetBloodIssueWorklist>, long>(Enumerable.Empty<GetBloodIssueWorklist>(), entity.Item2);
            }

            var activeIssues = (await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetAsync(x => x.IsActive == true && x.IsDelete == false && requestIds.Contains(x.BloodRequestId)))
                .ToList();

            var issuedRequestIds = activeIssues.Select(x => x.BloodRequestId).ToHashSet();
            var pendingRequests = requests.Where(x => !issuedRequestIds.Contains(x.Id)).ToList();
            var pendingRequestIds = pendingRequests.Select(x => x.Id).ToList();

            if (pendingRequestIds.Count == 0)
            {
                return new Tuple<IEnumerable<GetBloodIssueWorklist>, long>(Enumerable.Empty<GetBloodIssueWorklist>(), entity.Item2);
            }

            var crossMatches = (await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetAsync(
                    x => x.IsActive == true && x.IsDelete == false
                        && pendingRequestIds.Contains(x.BloodRequestId)
                        && x.Result == (int)BloodCrossMatchResult.Compatible,
                    includeProperties: "BloodUnit,BloodUnit.BloodComponentType,BloodUnit.BloodGroupMaster,BloodUnit.BloodFridge,BloodUnit.BloodRack"))
                .ToList();

            var crossMatchByRequest = crossMatches
                .GroupBy(x => x.BloodRequestId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First());

            var worklist = pendingRequests
                .Where(x => crossMatchByRequest.ContainsKey(x.Id))
                .Select(bloodRequest =>
                {
                    var crossMatch = crossMatchByRequest[bloodRequest.Id];
                    return new GetBloodIssueWorklist
                    {
                        BloodRequestId = bloodRequest.Id,
                        BloodRequest = mapper.Map<GetBloodRequest>(bloodRequest),
                        BloodCrossMatchId = crossMatch.Id,
                        CrossMatchDate = crossMatch.CrossMatchDate,
                        BloodUnitId = crossMatch.BloodUnitId,
                        BloodUnit = mapper.Map<GetBloodUnit>(crossMatch.BloodUnit)
                    };
                }).ToList();

            return new Tuple<IEnumerable<GetBloodIssueWorklist>, long>(worklist, worklist.Count);
        }
    }
}
