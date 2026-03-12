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
    public class GetAllGRNHandler : IRequestHandler<GetAllGRNQuery, Tuple<IEnumerable<GetGRN>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllGRNHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        //public async Task<Tuple<IEnumerable<GetGRN>, long>> Handle(GetAllGRNQuery request, CancellationToken cancellationToken)
        //{
        //    string[] roles = this.sessionProvider.Session.Roles;
        //    Expression<Func<Entities.Models.GRN, bool>> predicate;
        //    Expression<Func<Entities.Models.GRN, object>>[] includes = {
        //        x => x.CreatedBy,
        //        x => x.ProcessedBy,
        //        x => x.ApprovedBy,
        //        x => x.InvoiceApprovedBy,
        //        x => x.ModifiedBy,
        //        x => x.CreatedBy.Department.Company,
        //        x => x.Status,
        //        x => x.InvoiceStatus,
        //        x => x.Inspection,
        //        x => x.Inspection.IGP,
        //        x => x.Inspection.IGP.PurchaseOrder,
        //        x => x.Inspection.IGP.PurchaseOrder.Vendor,
        //        x => x.GRNDetail.Where(y => y.IsActive == true), // Keep only active details
        //    };

        //    List<string> thenIncludes = new()
        //    {
        //        "GRNDetail.CostSheet",
        //        "GRNDetail.Section",
        //        "GRNDetail.Section.Row",
        //        "GRNDetail.Section.Row.Rack",
        //        "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail",
        //        "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder",
        //        "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail",
        //        "GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item"
        //    };

        //    // Check if the current user's RoleId array contains the AccountOwnerRoleId
        //    if (roles.Contains("Store Manager"))
        //    {
        //        predicate = x => 
        //              x.IsActive == true
        //              && x.StatusId == request.StatusId
        //              && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
        //              && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
        //              && x.CreatedDate >= request.FDate.Value
        //              && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
        //              && (request.VendorId == null ||
        //               x.GRNDetail.Any(d =>
        //               d.IsActive &&
        //               d.InspectionDetail != null &&
        //               d.InspectionDetail.IGPDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
        //              && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
        //    }
        //    else if (roles.Contains("Store Issuer"))
        //    {
        //        if (request.StatusId == 1)
        //        {
        //            predicate = x => x.IsActive == true
        //              && x.StatusId == request.StatusId
        //              && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
        //                && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
        //              && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
        //              && x.CreatedDate >= request.FDate.Value
        //              && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
        //             && (request.VendorId == null ||
        //               x.GRNDetail.Any(d =>
        //               d.IsActive &&
        //               d.InspectionDetail != null &&
        //               d.InspectionDetail.IGPDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
        //              && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
        //        }
        //        else
        //        {
        //            predicate = x => x.IsActive == true
        //            && x.StatusId == request.StatusId
        //            && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
        //              && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
        //            && x.CreatedDate >= request.FDate.Value
        //            && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
        //           && (request.VendorId == null ||
        //               x.GRNDetail.Any(d =>
        //               d.IsActive &&
        //               d.InspectionDetail != null &&
        //               d.InspectionDetail.IGPDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
        //               d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
        //            && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
        //        }
        //    }
        //    else
        //    {
        //        predicate = x => x.IsActive == true
        //              && x.StatusId == request.StatusId
        //                && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
        //              && x.CreatedDate >= request.FDate.Value
        //              && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
        //              && (request.VendorId == null || x.GRNDetail.Where(d => d.IsActive).Any(d => d.InspectionDetail != null
        //              && d.InspectionDetail.IGPDetail != null
        //              && d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null
        //              && d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null
        //              && d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
        //              && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
        //    }

        //    Expression<Func<Entities.Models.GRN, object>> OrderBy = null;
        //    Expression<Func<Entities.Models.GRN, object>> OrderByDesc = x => x.Id;
        //    var entity = unitOfWork.Repository<Entities.Models.GRN>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

        //    var GRN = mapper.Map<IEnumerable<GetGRN>>(entity.Item1.ToList()).ToList();
        //    return new Tuple<IEnumerable<GetGRN>, long>(GRN, entity.Item2);
        //}

        public async Task<Tuple<IEnumerable<GetGRN>, long>> Handle(GetAllGRNQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.GRN, bool>> predicate;

            // Ensure roles is not null and handle null cases
            roles = roles ?? Array.Empty<string>();

            // Define the predicate based on roles with proper null checks
            if (roles.Contains("Store Manager"))
            {
                predicate = x =>
                      x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == null ||
                       x.GRNDetail.Any(d =>
                       d.IsActive &&
                       d.InspectionDetail != null &&
                       d.InspectionDetail.IGPDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Store Issuer"))
            {
                if (request.StatusId == 1)
                {
                    predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                        && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                     && (request.VendorId == null ||
                       x.GRNDetail.Any(d =>
                       d.IsActive &&
                       d.InspectionDetail != null &&
                       d.InspectionDetail.IGPDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
                }
                else
                {
                    predicate = x => x.IsActive == true
                    && x.StatusId == request.StatusId
                    && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                    && x.CreatedDate >= request.FDate.Value
                    && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                   && (request.VendorId == null ||
                       x.GRNDetail.Any(d =>
                       d.IsActive &&
                       d.InspectionDetail != null &&
                       d.InspectionDetail.IGPDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null &&
                       d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
                    && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
                }
            }
            else
            {
                // Default predicate for other roles
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == null || x.GRNDetail.Where(d => d.IsActive).Any(d => d.InspectionDetail != null
                      && d.InspectionDetail.IGPDetail != null
                      && d.InspectionDetail.IGPDetail.PurchaseOrderDetail != null
                      && d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder != null
                      && d.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder.VendorId == request.VendorId))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            // Add null checks for required parameters
            if (request.FDate == null || request.TDate == null)
            {
                throw new ArgumentException("From date and To date are required");
            }

            Expression<Func<Entities.Models.GRN, object>>[] includes = {
                    x => x.CreatedBy,
                    x => x.ProcessedBy,
                    x => x.ApprovedBy,
                    x => x.InvoiceApprovedBy,
                    x => x.ModifiedBy,
                    x => x.CreatedBy.Department.Company,
                    x => x.Status,
                    x => x.InvoiceStatus,
                    x => x.Inspection,
                    x => x.Inspection.IGP,
                    x => x.Inspection.IGP.PurchaseOrder,
                    x => x.Inspection.IGP.PurchaseOrder.Vendor,
                };

            Expression<Func<Entities.Models.GRN, object>> OrderByDesc = x => x.Id;

            // Get GRNs without GRNDetail
            var entity = unitOfWork.Repository<Entities.Models.GRN>().GetPagingWhereAsNoTrackingAsync(
                predicate, request.PagingData, null, OrderByDesc, null, includes);

            var grnIds = entity.Item1.Select(x => x.Id).ToList();

            if (!grnIds.Any())
            {
                // No GRNs found, return empty result
                return new Tuple<IEnumerable<GetGRN>, long>(Enumerable.Empty<GetGRN>(), 0);
            }

            // Second query: Get GRNDetails with all the nested includes
            Expression<Func<Entities.Models.GRNDetail, bool>> grnDetailPredicate = x => x.IsActive && grnIds.Contains(x.GRNId);

            var grnDetailsResult = unitOfWork.Repository<Entities.Models.GRNDetail>()
                .GetPagingWhereAsNoTrackingAsync(
                    predicate: grnDetailPredicate,
                    paging: null, // Get all details without paging
                    OrderBy: null,
                    OrderByDesc: null,
                    ThenIncludes: new List<string> {
                    "CostSheet",
                    "Section",
                    "Section.Row",
                    "Section.Row.Rack",
                    "InspectionDetail",
                    "InspectionDetail.IGPDetail",
                    "InspectionDetail.IGPDetail.PurchaseOrderDetail",
                    "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseOrder",
                    "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail",
                    "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item"},
                    includes: null
                );

            var grnDetails = grnDetailsResult.Item1.ToList();
            var grnDetailsLookup = grnDetails.ToLookup(x => x.GRNId);
            var grnList = entity.Item1.ToList();

            foreach (var grn in grnList)
            {
                grn.GRNDetail = grnDetailsLookup[grn.Id].ToList();
            }

            var GRN = mapper.Map<IEnumerable<GetGRN>>(grnList).ToList();
            return new Tuple<IEnumerable<GetGRN>, long>(GRN, entity.Item2);
        }

    }
}
