using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetAllPurchaseInvoiceHandler : IRequestHandler<GetAllPurchaseInvoiceQuery, Tuple<IEnumerable<GetGRN>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPurchaseInvoiceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetGRN>, long>> Handle_Old(GetAllPurchaseInvoiceQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.GRN, bool>> predicate;
            Expression<Func<Entities.Models.GRN, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.InvoiceApprovedBy,
                x => x.InvoiceProcessedBy,
                x => x.InvoiceAuditVerifiedBy,
                x => x.InvoiceApprovedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.InvoiceStatus,
                x => x.GRNDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new()
            {
                "GRNDetail.CostSheet",
                "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail",
                "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor",
                "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail",
                "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant") || roles.Contains("Audit"))
            {
                predicate = x => x.IsActive == true && x.InvoiceNo != null
                    && x.InvoiceStatusId == request.StatusId
                    && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                    && x.ApprovedDate != null
                    && x.ApprovedDate >= request.FDate.Value
                    && x.ApprovedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                    && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                    && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode.Trim()))
                    && (request.Code == "" || x.InvoiceNo.ToLower().Contains(request.Code.Trim()));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.InvoiceNo != null
                      && x.InvoiceStatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                      && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.GRN, object>> OrderBy = null;
            Expression<Func<Entities.Models.GRN, object>> OrderByDesc = x => x.ModifiedDate;
            var entity = unitOfWork.Repository<Entities.Models.GRN>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var GRN = mapper.Map<IEnumerable<GetGRN>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetGRN>, long>(GRN, entity.Item2);
        }

        public async Task<Tuple<IEnumerable<GetGRN>, long>> Handle(GetAllPurchaseInvoiceQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.GRN, bool>> predicate;

            // Ensure roles is not null and handle null cases
            roles = roles ?? Array.Empty<string>();

            // Define the predicate based on roles with proper null checks
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant") || roles.Contains("Audit"))
            {
                predicate = x => x.IsActive == true && x.InvoiceNo != null
                    && x.InvoiceStatusId == request.StatusId
                    && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                    && x.ApprovedDate != null
                    && x.ApprovedDate >= request.FDate.Value
                    && x.ApprovedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                    && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                    && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode.Trim()))
                    && (request.Code == "" || x.InvoiceNo.ToLower().Contains(request.Code.Trim()));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.InvoiceNo != null
                      && x.InvoiceStatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                      && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            // Parent includes for GRN entity (without child GRNDetails)
            Expression<Func<Entities.Models.GRN, object>>[] includes = {
                   x => x.CreatedBy,
                   x => x.ProcessedBy,
                   x => x.ApprovedBy,
                   x => x.InvoiceApprovedBy,
                   x => x.InvoiceProcessedBy,
                   x => x.InvoiceAuditVerifiedBy,
                   x => x.CreatedBy.Department.Company,
                   x => x.Status,
                   x => x.InvoiceStatus,
             };

            // Order by ModifiedDate descending (could be based on your requirements)
            Expression<Func<Entities.Models.GRN, object>> OrderByDesc = x => x.ModifiedDate;

            // Fetch the GRN entities (parent)
            var entity = unitOfWork.Repository<Entities.Models.GRN>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, null, OrderByDesc, null, includes);

            var grnList = entity.Item1.ToList();
            var grnIds = grnList.Select(x => x.Id).ToList();

            if (!grnIds.Any())
            {
                // No GRNs found, return empty result
                return new Tuple<IEnumerable<GetGRN>, long>(Enumerable.Empty<GetGRN>(), 0);
            }

            // Second query: Get the related GRNDetails (child) for the GRNs
            Expression<Func<Entities.Models.GRNDetail, bool>> grnDetailPredicate = x => x.IsActive && grnIds.Contains(x.GRNId);
            var grnDetailsResult = unitOfWork.Repository<Entities.Models.GRNDetail>()
                .GetPagingWhereAsNoTrackingAsync(
                    grnDetailPredicate,
                    paging: null, // No paging, get all details
                    OrderBy: null,
                    OrderByDesc: null,
                    ThenIncludes: new List<string>
                    {
                     "CostSheet",
                     "InspectionDetail",
                     "InspectionDetail.IGPDetail",
                     "InspectionDetail.IGPDetail.PurchaseOrderDetail",
                     "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder",
                     "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.Vendor",
                     "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail",
                     "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item"
                    },
                    includes: null
                );

            var grnDetails = grnDetailsResult.Item1.ToList();
            var grnDetailsLookup = grnDetails.ToLookup(x => x.GRNId);

            // Attach the GRNDetails to their respective GRN
            foreach (var grn in grnList)
            {
                grn.GRNDetail = grnDetailsLookup[grn.Id].ToList();
            }

            // Map the GRNs to the GetGRN model
            var GRN = mapper.Map<IEnumerable<GetGRN>>(grnList).ToList();

            // Return the GRN list along with the total count
            return new Tuple<IEnumerable<GetGRN>, long>(GRN, entity.Item2);
        }

    }
}
