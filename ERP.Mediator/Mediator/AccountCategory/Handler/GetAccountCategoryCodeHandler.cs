using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class GetAccountCategoryCodeHandler : IRequestHandler<GetAccountCategoryCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetAccountCategoryCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountCategoryCodeQuery request, CancellationToken cancellationToken)
        {
            string _AccountCategoryCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountCategory>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.AccountCategory>, IOrderedQueryable<Entities.Models.AccountCategory>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountCategoryCode = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(AccountCategoryCode.Code) + 1;
                _AccountCategoryCode = No.ToString().PadLeft(2, '0');
            }
            else
                _AccountCategoryCode = "01";
          
            return _AccountCategoryCode;
        }
    }
}
