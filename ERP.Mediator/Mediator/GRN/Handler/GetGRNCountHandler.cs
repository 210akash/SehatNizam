using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetGRNCountHandler : IRequestHandler<GetGRNCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetGRNCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetGRNCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.GRN, bool>> predicate;
            Expression<Func<Entities.Models.GRN, object>>[] includes = {
                x => x.Inspection,
                x => x.Inspection.IGP,
                x => x.Inspection.IGP.PurchaseOrder,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager"))
            {
                predicate = x => x.IsActive == true
                 && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                   && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true
                && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                   && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId

                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                   && x.Inspection.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                           && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.GRN>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = 0;
            if (roles.Contains("Purchase Manager"))
            {
                Created = entity.Item1.Count(item => item.StatusId == 1);
            }
            else if (roles.Contains("Purchaser"))
            {
                Created = entity.Item1.Count(item => item.StatusId == 1 && item.CreatedById == this.sessionProvider.Session.LoggedInUserId);
            }
            else
            {
                Created = entity.Item1.Count(item => item.StatusId == 1);
            }

            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
