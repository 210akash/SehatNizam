using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Request.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Handler
{
    public class GetBloodRequestLogHandler : IRequestHandler<GetBloodRequestLogQuery, GetBloodRequestLog>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBloodRequestLogHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetBloodRequestLog> Handle(GetBloodRequestLogQuery request, CancellationToken cancellationToken)
        {
            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(
                    x => x.Id == request.BloodRequestId && x.IsActive == true && x.IsDelete == false,
                    null,
                    null,
                    "BloodGroupMaster,BloodComponentType,CreatedBy");

            if (bloodRequest == null) return null;

            var crossMatches = (await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()
                .GetAsync(
                    x => x.BloodRequestId == request.BloodRequestId,
                    includeProperties: "BloodUnit,CreatedBy,ModifiedBy"))
                .OrderBy(x => x.CreatedDate)
                .ToList();

            var issues = (await unitOfWork.Repository<Entities.Models.BloodIssue>()
                .GetAsync(
                    x => x.BloodRequestId == request.BloodRequestId,
                    includeProperties: "BloodUnit,CreatedBy,ModifiedBy"))
                .OrderBy(x => x.CreatedDate)
                .ToList();

            var entries = new List<GetBloodRequestLogEntry>();
            entries.Add(new GetBloodRequestLogEntry
            {
                EventDate = bloodRequest.CreatedDate ?? bloodRequest.RequestDate,
                Step = "Blood Request",
                Description = $"Request {bloodRequest.Code} created — Status: Pending",
                Outcome = "Completed",
                PerformedBy = GetUserName(bloodRequest.CreatedBy)
            });

            foreach (var crossMatch in crossMatches)
            {
                var unitNo = crossMatch.BloodUnit?.UnitNo ?? crossMatch.BloodUnitId.ToString();
                entries.Add(new GetBloodRequestLogEntry
                {
                    EventDate = crossMatch.CreatedDate ?? crossMatch.CrossMatchDate,
                    Step = "Cross Match",
                    Description = $"Unit {unitNo} assigned for cross match",
                    Outcome = "In Process",
                    UnitNo = unitNo,
                    PerformedBy = GetUserName(crossMatch.CreatedBy),
                    IsReverted = crossMatch.IsDelete
                });

                if (crossMatch.Result != (int)BloodCrossMatchResult.InProcess)
                {
                    entries.Add(new GetBloodRequestLogEntry
                    {
                        EventDate = crossMatch.ModifiedDate ?? crossMatch.CrossMatchDate,
                        Step = "Cross Match Result",
                        Description = GetCrossMatchResultText(crossMatch.Result),
                        Outcome = crossMatch.Result == (int)BloodCrossMatchResult.Compatible ? "Compatible" : "Incompatible",
                        UnitNo = unitNo,
                        PerformedBy = GetUserName(crossMatch.ModifiedBy ?? crossMatch.CreatedBy),
                        IsReverted = crossMatch.IsDelete
                    });
                }

                if (crossMatch.IsDelete)
                {
                    entries.Add(new GetBloodRequestLogEntry
                    {
                        EventDate = crossMatch.DeleteDate ?? crossMatch.ModifiedDate ?? DateTime.Now,
                        Step = "Cross Match Reverted",
                        Description = $"Cross match cleared — request returned to cross match queue",
                        Outcome = "Reverted",
                        UnitNo = unitNo,
                        PerformedBy = GetUserName(crossMatch.ModifiedBy),
                        IsReverted = true
                    });
                }
            }

            foreach (var issue in issues)
            {
                var unitNo = issue.BloodUnit?.UnitNo ?? issue.BloodUnitId.ToString();
                entries.Add(new GetBloodRequestLogEntry
                {
                    EventDate = issue.IssueDate,
                    Step = "Blood Issue",
                    Description = $"Blood issued to {issue.IssuedTo}",
                    Outcome = "Issued",
                    UnitNo = unitNo,
                    PerformedBy = GetUserName(issue.CreatedBy),
                    IsReverted = issue.IsDelete
                });

                if (issue.IsDelete)
                {
                    entries.Add(new GetBloodRequestLogEntry
                    {
                        EventDate = issue.DeleteDate ?? issue.ModifiedDate ?? DateTime.Now,
                        Step = "Blood Issue Reverted",
                        Description = "Issue deleted — request returned to pending issue",
                        Outcome = "Reverted",
                        UnitNo = unitNo,
                        PerformedBy = GetUserName(issue.ModifiedBy),
                        IsReverted = true
                    });
                }
            }

            var log = new GetBloodRequestLog
            {
                BloodRequest = mapper.Map<GetBloodRequest>(bloodRequest),
                CurrentStep = GetCurrentStep(bloodRequest, crossMatches, issues),
                Entries = entries.OrderByDescending(x => x.EventDate).ToList()
            };

            return log;
        }

        private static string GetCurrentStep(
            Entities.Models.BloodRequest request,
            List<Entities.Models.BloodCrossMatch> crossMatches,
            List<Entities.Models.BloodIssue> issues)
        {
            if (request.Status == (int)BloodRequestStatus.Cancelled) return "Cancelled";
            if (request.Status == (int)BloodRequestStatus.Issued) return "Blood Issued — Completed";

            var activeIssue = issues.FirstOrDefault(x => x.IsActive && !x.IsDelete);
            if (activeIssue != null) return "Blood Issued";

            if (request.Status == (int)BloodRequestStatus.CrossMatched)
                return "Cross Matched — Awaiting Blood Issue";

            var inProcess = crossMatches.FirstOrDefault(x => x.IsActive && !x.IsDelete && x.Result == (int)BloodCrossMatchResult.InProcess);
            if (inProcess != null) return "Cross Match In Process";

            return "Pending — Awaiting Cross Match";
        }

        private static string GetCrossMatchResultText(int result)
        {
            return result switch
            {
                (int)BloodCrossMatchResult.Compatible => "Cross match compatible — unit reserved for patient",
                (int)BloodCrossMatchResult.Incompatible => "Cross match incompatible — unit released",
                _ => "Cross match in process"
            };
        }

        private static string GetUserName(Entities.Models.AspNetUsers user)
        {
            if (user == null) return "";
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return !string.IsNullOrWhiteSpace(fullName) ? fullName : user.UserName ?? "";
        }
    }
}
