using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class GetAllPurchaseReturnHandler : IRequestHandler<GetAllPurchaseReturnQuery, Tuple<IEnumerable<GetPurchaseReturn>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPurchaseReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPurchaseReturn>, long>> Handle(GetAllPurchaseReturnQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;

            Expression<Func<Entities.Models.PurchaseReturn, bool>> predicate;
            Expression<Func<Entities.Models.PurchaseReturn, object>>[] includes = {
        x => x.CreatedBy,
        x => x.ModifiedBy,
        x => x.ProcessedBy,
        x => x.ApprovedBy,
        x => x.Project,
        x => x.CreatedBy.Department.Company,
        x => x.Status,
        x => x.GRN,
        x => x.GRN.Inspection,
        x => x.GRN.Inspection.IGP,
        x => x.GRN.Inspection.IGP.PurchaseOrder,
        x => x.GRN.Inspection.IGP.PurchaseOrder.Vendor,
        x => x.PurchaseReturnDetail.Where(y => y.IsActive == true),
    };

            List<string> thenIncludes = new()
    {
        "PurchaseReturnDetail",
        "PurchaseReturnDetail.GRNDetail",
        "PurchaseReturnDetail.GRNDetail.InspectionDetail",
        "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail",
        "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail",
        "PurchaseReturnDetail.GRNDetail.InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item",
    };

            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == 0 || x.GRN.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.VendorId == 0 || x.GRN.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.PurchaseReturn, object>> OrderBy = null;
            Expression<Func<Entities.Models.PurchaseReturn, object>> OrderByDesc = x => x.Id;

            // ✅ Fix: Await the async call
            var entity =  unitOfWork.Repository<Entities.Models.PurchaseReturn>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var source = entity.Item1 ?? Enumerable.Empty<Entities.Models.PurchaseReturn>();
            var purchaseReturn = mapper.Map<IEnumerable<GetPurchaseReturn>>(source);
            return new Tuple<IEnumerable<GetPurchaseReturn>, long>(purchaseReturn, entity.Item2);
        }
    }
}
