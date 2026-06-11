using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;

using System.Threading;

using System.Threading.Tasks;

using AutoMapper;

using ERP.BusinessModels.Enums;

using ERP.BusinessModels.ResponseVM;

using ERP.Mediator.Mediator.BloodBank.CrossMatch.Query;

using ERP.Repositories.UnitOfWork;

using MediatR;



namespace ERP.Mediator.Mediator.BloodBank.CrossMatch.Handler

{

    public class GetBloodCrossMatchWorklistHandler : IRequestHandler<GetBloodCrossMatchWorklistQuery, Tuple<IEnumerable<GetBloodCrossMatchWorklist>, long>>

    {

        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;



        public GetBloodCrossMatchWorklistHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {

            this.unitOfWork = unitOfWork;

            this.mapper = mapper;

        }



        public async Task<Tuple<IEnumerable<GetBloodCrossMatchWorklist>, long>> Handle(GetBloodCrossMatchWorklistQuery request, CancellationToken cancellationToken)

        {

            Expression<Func<Entities.Models.BloodRequest, bool>> predicate = x => x.IsActive == true && x.IsDelete == false

                && x.Status == (int)BloodRequestStatus.Pending

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



            var crossMatches = requestIds.Count == 0

                ? new List<Entities.Models.BloodCrossMatch>()

                : (await unitOfWork.Repository<Entities.Models.BloodCrossMatch>()

                    .GetAsync(

                        x => x.IsActive == true && x.IsDelete == false

                            && requestIds.Contains(x.BloodRequestId)

                            && x.Result == (int)BloodCrossMatchResult.InProcess,

                        includeProperties: "BloodUnit,BloodUnit.BloodComponentType,BloodUnit.BloodFridge,BloodUnit.BloodRack")).ToList();



            var crossMatchByRequest = crossMatches

                .GroupBy(x => x.BloodRequestId)

                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First());



            var worklist = requests.Select(bloodRequest =>

            {

                crossMatchByRequest.TryGetValue(bloodRequest.Id, out var crossMatch);

                var mappedRequest = mapper.Map<GetBloodRequest>(bloodRequest);



                if (crossMatch == null)

                {

                    return new GetBloodCrossMatchWorklist

                    {

                        CrossMatchId = 0,

                        BloodRequestId = bloodRequest.Id,

                        BloodRequest = mappedRequest,

                        BloodUnitId = 0,

                        Result = 0

                    };

                }



                return new GetBloodCrossMatchWorklist

                {

                    CrossMatchId = crossMatch.Id,

                    BloodRequestId = bloodRequest.Id,

                    BloodRequest = mappedRequest,

                    BloodUnitId = crossMatch.BloodUnitId,

                    BloodUnit = mapper.Map<GetBloodUnit>(crossMatch.BloodUnit),

                    CrossMatchDate = crossMatch.CrossMatchDate,

                    Result = crossMatch.Result,

                    Remarks = crossMatch.Remarks

                };

            }).ToList();



            return new Tuple<IEnumerable<GetBloodCrossMatchWorklist>, long>(worklist, entity.Item2);

        }

    }

}


