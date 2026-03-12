using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetAllInspectionHandler : IRequestHandler<GetAllInspectionQuery, Tuple<IEnumerable<GetInspection>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllInspectionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetInspection>, long>> Handle(GetAllInspectionQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Inspection, bool>> predicate;
            Expression<Func<Entities.Models.Inspection, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ModifiedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.IGP,
                x => x.IGP.PurchaseOrder,
                x => x.InspectionDetail.Where(y => y.IsActive == true)
            };

            List<string> thenIncludes = new()
            {
                "InspectionDetail.IGPDetail",
                "InspectionDetail.RejectReason",
                "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail",
                "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item",
                "InspectionDetail.IGPDetail.PurchaseOrderDetail.PurchaseDemandDetail.Item.UOM"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Inspection"))
            {
                predicate = x => x.IsActive == true
                   && x.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                   && x.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      //&& x.InspectionDetail.Where(y => y.IsActive == true) 
                      //.Any(d => d.InspectionVendor.Any(v => v.IsActive == true))
                      //&& (request.Code == "" || x.Code.ToLower().Contains(request.Code))
                      ;
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.Inspection, object>> OrderBy = null;
            Expression<Func<Entities.Models.Inspection, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Inspection>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var Inspection = mapper.Map<IEnumerable<GetInspection>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetInspection>, long>(Inspection, entity.Item2);
        }
    }
}
