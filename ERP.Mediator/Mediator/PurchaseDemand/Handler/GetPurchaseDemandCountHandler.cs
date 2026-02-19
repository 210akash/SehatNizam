using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPurchaseDemandCountHandler : IRequestHandler<GetPurchaseDemandCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetPurchaseDemandCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetPurchaseDemandCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PurchaseDemand, bool>> predicate;
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager"))
            {
                predicate = x => x.IsActive == true
                //&& x.DepartmentId == this.sessionProvider.Session.DepartmentId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true 
                //&& x.DepartmentId == this.sessionProvider.Session.DepartmentId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            var entity = unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
            int Created = entity.Item1.Count(item => item.StatusId == 1 && item.CreatedById == this.sessionProvider.Session.LoggedInUserId);
            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
