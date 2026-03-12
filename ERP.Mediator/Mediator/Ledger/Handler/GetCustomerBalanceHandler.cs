using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Ledger.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Ledger.Handler
{
    public class GetCustomerBalanceHandler : IRequestHandler<GetCustomerBalanceQuery, decimal>
    {
        private readonly IUnitOfWorkDapper unitOfWorkDapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetCustomerBalanceHandler(IUnitOfWork unitOfWork,  IUnitOfWorkDapper unitOfWorkDapper, SessionProvider sessionProvider)
        {
            this.unitOfWorkDapper = unitOfWorkDapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<decimal> Handle(GetCustomerBalanceQuery request, CancellationToken cancellationToken)
        {
            var accountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>()
                               .GetFirstAsNoTrackingAsync(y => y.DealershipId == request.CustomerId);

            var reportQuery = $"OpeningBalanceLedgerSP @TillDate = '{DateTime.Now.Date}'," +
                              $"@CompanyId = '{sessionProvider.Session.CompanyId}'," +
                              $"@Account = '{accountGroup?.Code}'";

            var result = await unitOfWorkDapper.Repository<CustomerBalanceResponse>()
                .QueryAsync<CustomerBalanceResponse>(reportQuery);

            return result.FirstOrDefault()?.OpeningBalance ?? 0;
        }

        public class CustomerBalanceResponse
        {
            public decimal OpeningBalance { get; set; }
        }
    }
}
