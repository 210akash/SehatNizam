using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class GetShopOrderReturnCountHandler : IRequestHandler<GetShopOrderReturnCountQuery, Tuple<long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetShopOrderReturnCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long>> Handle(GetShopOrderReturnCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.ShopOrderReturn, bool>> predicate;
            Expression<Func<Entities.Models.ShopOrderReturn, object>>[] includes = {
                x => x.ShopOrder,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                // && x.DispatchOrder.Order.Com == this.sessionProvider.Session.CompanyId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.ShopOrderReturnId == "" || x.ShopOrderId.ToString().Contains(request.ShopOrderReturnId))
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.ShopOrderReturnId == "" || x.ShopOrderId.ToString().Contains(request.ShopOrderReturnId))
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = entity.Item1.Count(item => item.StatusId == 1);
            int Post = entity.Item1.Count(item => item.StatusId == 3);
            return new Tuple<long, long>(Created, Post);
        }
    }
}
