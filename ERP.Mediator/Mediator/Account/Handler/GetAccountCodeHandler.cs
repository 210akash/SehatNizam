using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class GetAccountCodeHandler : IRequestHandler<GetAccountCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetAccountCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountCodeQuery request, CancellationToken cancellationToken)
        {
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountTypeId);
            string _AccountCode = "";
            if (await unitOfWork.Repository<Entities.Models.Account>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.AccountTypeId == request.AccountTypeId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.Account>, IOrderedQueryable<Entities.Models.Account>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountCode = await unitOfWork.Repository<Entities.Models.Account>().GetOneAsync(y => y.IsActive == true && y.AccountTypeId == request.AccountTypeId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(AccountCode.Code.TakeLast(4).ToArray())) + 1;
                _AccountCode = No.ToString().PadLeft(4, '0');
            }
            else
                _AccountCode = "0001";
          
            return AccountType.Code + _AccountCode;
        }
    }
}
