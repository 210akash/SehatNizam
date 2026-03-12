using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetPurchaseDemandCodeHandler : IRequestHandler<GetPurchaseDemandCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetPurchaseDemandCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetPurchaseDemandCodeQuery request, CancellationToken cancellationToken)
        {
            string _PurchaseDemandCode = "";
            if (await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.PurchaseDemand>, IOrderedQueryable<Entities.Models.PurchaseDemand>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var PurchaseDemandCode = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(PurchaseDemandCode.Code) + 1;
                _PurchaseDemandCode = No.ToString().PadLeft(7, '0');
            }
            else
                _PurchaseDemandCode = "0000001";
          
            return _PurchaseDemandCode;
        }
    }
}
