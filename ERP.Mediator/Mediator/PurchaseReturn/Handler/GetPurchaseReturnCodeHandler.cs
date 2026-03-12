using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class GetPurchaseReturnCodeHandler : IRequestHandler<GetPurchaseReturnCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetPurchaseReturnCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetPurchaseReturnCodeQuery request, CancellationToken cancellationToken)
        {
            string _PurchaseReturnCode = "";
            if (await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetExistsAsync(y => y.IsActive))
            {
                Func<IQueryable<Entities.Models.PurchaseReturn>, IOrderedQueryable<Entities.Models.PurchaseReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var PurchaseReturnCode = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(PurchaseReturnCode.Code) + 1;
                _PurchaseReturnCode = No.ToString().PadLeft(7, '0');
            }
            else
                _PurchaseReturnCode = "0000001";
          
            return _PurchaseReturnCode;
        }
    }
}
