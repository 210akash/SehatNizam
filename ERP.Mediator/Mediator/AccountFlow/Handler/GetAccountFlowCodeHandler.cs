using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountFlow.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountFlow.Handler
{
    public class GetAccountFlowCodeHandler : IRequestHandler<GetAccountFlowCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetAccountFlowCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountFlowCodeQuery request, CancellationToken cancellationToken)
        {
            string _AccountFlowCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountFlow>().GetExistsAsync(y => y.CompanyId == sessionProvider.Session.CompanyId && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.AccountFlow>, IOrderedQueryable<Entities.Models.AccountFlow>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountFlowCode = await unitOfWork.Repository<Entities.Models.AccountFlow>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(AccountFlowCode.Code) + 1;
                _AccountFlowCode = No.ToString().PadLeft(2, '0');
            }
            else
                _AccountFlowCode = "01";
          
            return _AccountFlowCode;
        }
    }
}
