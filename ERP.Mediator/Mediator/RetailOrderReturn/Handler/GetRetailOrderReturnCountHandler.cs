using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class GetRetailOrderReturnCountHandler : IRequestHandler<GetRetailOrderReturnCountQuery, Tuple<long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetRetailOrderReturnCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long>> Handle(GetRetailOrderReturnCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.RetailOrderReturn, bool>> predicate;
            Expression<Func<Entities.Models.RetailOrderReturn, object>>[] includes = {
                x => x.RetailOrder,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                          && (x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId)
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (string.IsNullOrEmpty(request.RetailOrderId) || x.RetailOrderId.ToString().Contains(request.RetailOrderId))
                          && x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                          && (x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId)
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (string.IsNullOrEmpty(request.RetailOrderId) || x.RetailOrderId.ToString().Contains(request.RetailOrderId))
                          && x.RetailOrder.ShopId == sessionProvider.Session.RetailUserShopId
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = entity.Item1.Count(item => item.StatusId == 1);
            int Post = entity.Item1.Count(item => item.StatusId == 3);
            return new Tuple<long, long>(Created, Post);
        }
    }
}
