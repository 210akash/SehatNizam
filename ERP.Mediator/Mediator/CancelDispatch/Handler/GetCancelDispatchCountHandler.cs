using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CancelDispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Stripe;

namespace ERP.Mediator.Mediator.CancelDispatch.Handler
{
    public class GetDispatchCountHandler : IRequestHandler<GetCancelDispatchCountQuery, Tuple<long, long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetDispatchCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long, long>> Handle(GetCancelDispatchCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.CancelDispatch, bool>> predicate;

            predicate = x => x.IsActive == true
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));

            var entity = unitOfWork.Repository<Entities.Models.CancelDispatch>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
            
            int Created = entity.Item1.Count(item => item.StatusId == (long)OrderStatusEnum.CancelDispatchCreated);
            int Forwarded = entity.Item1.Count(item => item.StatusId == (long)OrderStatusEnum.CancelDispatchForward);
            int SalesReviewed = entity.Item1.Count(item => item.StatusId == (long)OrderStatusEnum.CancelDispatchSalesReviewed);
            int AccountReviewed = entity.Item1.Count(item => item.StatusId == (long)OrderStatusEnum.CancelDispatchAccountReviewed);
            int Confirmed = entity.Item1.Count(item => item.StatusId == (long)OrderStatusEnum.CancelDispatchConfirm);
            return new Tuple<long, long, long, long, long>(Created, Forwarded, SalesReviewed, AccountReviewed, Confirmed);
        }
    }
}
