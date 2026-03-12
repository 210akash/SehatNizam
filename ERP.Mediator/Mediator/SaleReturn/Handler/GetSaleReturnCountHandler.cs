using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class GetSaleReturnCountHandler : IRequestHandler<GetSaleReturnCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetSaleReturnCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetSaleReturnCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SaleReturn, bool>> predicate;
            Expression<Func<Entities.Models.SaleReturn, object>>[] includes = {
                x => x.DispatchOrder,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                 && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.DealershipId == 0 || x.DispatchOrder.Order.DealershipId == request.DealershipId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                 && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.DealershipId == 0 || x.DispatchOrder.Order.DealershipId == request.DealershipId)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.SaleReturn>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = entity.Item1.Count(item => item.StatusId == 1);
            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
