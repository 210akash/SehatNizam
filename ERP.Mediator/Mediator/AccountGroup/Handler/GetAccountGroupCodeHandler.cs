using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountGroup.Handler
{
    public class GetAccountGroupCodeHandler : IRequestHandler<GetAccountGroupCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetAccountGroupCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountGroupCodeQuery request, CancellationToken cancellationToken)
        {
            var AccountGroupType = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.Id == request.AccountId);
            string _AccountGroupCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountGroup>().GetExistsAsync(y =>  y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.AccountId == request.AccountId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.AccountGroup>, IOrderedQueryable<Entities.Models.AccountGroup>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountGroupCode = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetOneAsync(y => y.IsActive == true && y.AccountId == request.AccountId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(AccountGroupCode.Code.TakeLast(4).ToArray())) + 1;
                _AccountGroupCode = No.ToString().PadLeft(4, '0');
            }
            else
                _AccountGroupCode = "0001";
          
            return AccountGroupType.Code + _AccountGroupCode;
        }
    }
}
