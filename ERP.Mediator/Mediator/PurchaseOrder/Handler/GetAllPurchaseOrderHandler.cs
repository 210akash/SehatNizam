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
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetAllPurchaseOrderHandler : IRequestHandler<GetAllPurchaseOrderQuery, Tuple<IEnumerable<GetPurchaseOrder>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPurchaseOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPurchaseOrder>, long>> Handle(GetAllPurchaseOrderQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PurchaseOrder, bool>> predicate;
            Expression<Func<Entities.Models.PurchaseOrder, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Vendor,
                x => x.Currency,
                x => x.ShipmentMode,
                x => x.PaymentMode,
                x => x.DeliveryTerms,
                x => x.Company,
                x => x.Status,
                x => x.PurchaseOrderDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new()
            {
                "PurchaseOrderDetail.PurchaseDemandDetail",
                "PurchaseOrderDetail.PurchaseDemandDetail.PurchaseDemand",
                "PurchaseOrderDetail.PurchaseDemandDetail.Project",
                "PurchaseOrderDetail.PurchaseDemandDetail.Department",
                "PurchaseOrderDetail.PurchaseDemandDetail.Item",
                "PurchaseOrderDetail.PurchaseDemandDetail.Item.UOM",
                "PurchaseOrderDetail.ComparativeStatementVendor"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                      && x.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.PurchaseOrder, object>> OrderBy = null;
            Expression<Func<Entities.Models.PurchaseOrder, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var PurchaseOrder = mapper.Map<IEnumerable<GetPurchaseOrder>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetPurchaseOrder>, long>(PurchaseOrder, entity.Item2);
        }
    }
}
