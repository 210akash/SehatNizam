using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class GetPurchaseReturnCountHandler : IRequestHandler<GetPurchaseReturnCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetPurchaseReturnCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetPurchaseReturnCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PurchaseReturn, bool>> predicate;
            Expression<Func<Entities.Models.PurchaseReturn, object>>[] includes = {
                x => x.GRN,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                          && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == 0 || x.GRN.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                 && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == 0 || x.GRN.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = entity.Item1.Count(item => item.StatusId == 1);
            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
