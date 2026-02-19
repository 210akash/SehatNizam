using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountHead.Handler
{
    public class GetAccountHeadCodeHandler : IRequestHandler<GetAccountHeadCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetAccountHeadCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountHeadCodeQuery request, CancellationToken cancellationToken)
        {
            string _AccountHeadCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountHead>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.AccountHead>, IOrderedQueryable<Entities.Models.AccountHead>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountHeadCode = await unitOfWork.Repository<Entities.Models.AccountHead>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(AccountHeadCode.Code) + 1;
                _AccountHeadCode = No.ToString().PadLeft(2, '0');
            }
            else
                _AccountHeadCode = "01";
          
            return _AccountHeadCode;
        }
    }
}
