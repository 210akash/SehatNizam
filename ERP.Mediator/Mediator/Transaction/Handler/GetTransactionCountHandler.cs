using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class GetTransactionCountHandler : IRequestHandler<GetTransactionCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetTransactionCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetTransactionCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Transaction, bool>> predicate;
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                  && x.VoucherTypeId == request.VoucherTypeId
                          && x.Date.Date >= request.FDate.Value
                          && x.Date.Date <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                  && x.VoucherTypeId == request.VoucherTypeId
                          && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                          && x.Date.Date >= request.FDate.Value
                          && x.Date.Date <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                          && x.Date.Date >= request.FDate.Value
                          && x.Date.Date <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

                var entity = unitOfWork.Repository<Entities.Models.Transaction>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
            int Created = entity.Item1.Count(item => item.StatusId == 1 );
            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
