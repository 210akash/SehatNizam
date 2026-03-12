using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class GetPendingDemandHandler : IRequestHandler<GetPendingDemandQuery, List<GetDropDown>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetPendingDemandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetDropDown>> Handle(GetPendingDemandQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<PurchaseDemandDetail, bool>> predicate;
            Expression<Func<PurchaseDemandDetail, object>>[] includes = {
                x => x.CreatedBy,
                x => x.PurchaseDemand,
                x => x.ComparativeStatementDetail.ComparativeStatementVendor.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            if (request.ComparativeStatementId == 0)
            {
                // Case: Exclude all PurchaseDemandDetails already linked to ComparativeStatementDetail
                predicate = x => x.IsActive &&
                                        // x.PurchaseDemand.StatusId == 3 &&
                                         !x.ComparativeStatementDetail.ComparativeStatementVendor.Any(cs => cs.IsActive); // Exclude linked details
            }
            else
            {
                // Case: Include the specified PurchaseDemandId and filter others not linked to ComparativeStatementDetail
                predicate =x => x.IsActive &&
                                        // x.PurchaseDemand.StatusId == 3 &&
                                         (!x.ComparativeStatementDetail.ComparativeStatementVendor.Any(cs => cs.IsActive) || x.PurchaseDemandId == request.PurchaseDemandId);
            }

            Expression<Func<PurchaseDemandDetail, object>> OrderByDesc = x => x.PurchaseDemand.CreatedDate;
            var entity = unitOfWork.Repository<PurchaseDemandDetail>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, OrderByDesc, null, includes);
            var pendingDemandDetails = entity.Item1.ToList();

            // Filter distinct PurchaseDemands and map to DTO
            var distinctPurchaseDemands = pendingDemandDetails
                .DistinctBy(detail => detail.PurchaseDemandId)
                .Select(detail => detail.PurchaseDemand)
                .ToList();

            // Map to GetDropDown DTOs
            return mapper.Map<List<GetDropDown>>(distinctPurchaseDemands);
        }
    }
}
