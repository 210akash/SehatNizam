using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetPendingIGPsHandler : IRequestHandler<GetPendingIGPsQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetPendingIGPsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<List<GetDropDown>> Handle(GetPendingIGPsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.IGP, object>> orderByExpression = x => x.CreatedDate;
            var IGPs = await unitOfWork.Repository<Entities.Models.IGP>().GetAsync(
                x => x.IsActive &&
                     x.StatusId == 3 &&
                     x.ProjectId == sessionProvider.Session.SelectedWarehouseId,
                includeProperties: "IGPDetails,IGPDetails.PurchaseOrderDetail",
                orderByDec: query => query.OrderByDescending(orderByExpression)
            );

            // Step 1: Get all active InspectionDetails
            var allInspections = unitOfWork.Repository<InspectionDetail>()
                .GetAll()
                .Where(x => x.IsActive)
                .ToList();

            // Step 2: Get latest inspection (by InspectionId) for each IGPDetailId
            var latestInspections = allInspections
                .GroupBy(x => x.IGPDetailId)
                .Select(g => g.OrderByDescending(x => x.InspectionId).First())
                .ToDictionary(x => x.IGPDetailId, x => x.Rejected);

            List<Entities.Models.IGP> pendingIGP = new();

            foreach (var igp in IGPs)
            {
                bool isRequestedIGP = igp.Id == request.IGPId;

                // Check if any detail is still pending
                bool hasPendingDetails = igp.IGPDetails
                    .Where(d => d.IsActive)
                    .Any(d =>
                        // 1. No inspection yet for this detail
                        !latestInspections.ContainsKey(d.Id)
                        ||
                        // 2. Latest inspection has Rejected > 0
                        latestInspections[d.Id] > 0
                    );

                if (isRequestedIGP || hasPendingDetails)
                {
                    pendingIGP.Add(igp);
                }
            }

            return mapper.Map<List<GetDropDown>>(pendingIGP);
        }
    }
}
