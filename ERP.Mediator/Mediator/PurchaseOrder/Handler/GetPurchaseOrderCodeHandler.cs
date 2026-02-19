using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.PurchaseOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.PurchaseOrder.Handler
{
    public class GetPurchaseOrderCodeHandler : IRequestHandler<GetPurchaseOrderCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetPurchaseOrderCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetPurchaseOrderCodeQuery request, CancellationToken cancellationToken)
        {
            string _PurchaseOrderCode = "";
            if (await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.PurchaseOrder>, IOrderedQueryable<Entities.Models.PurchaseOrder>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var PurchaseOrderCode = await unitOfWork.Repository<Entities.Models.PurchaseOrder>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(PurchaseOrderCode.Code) + 1;
                _PurchaseOrderCode = No.ToString().PadLeft(7, '0');
            }
            else
                _PurchaseOrderCode = "0000001";
          
            return _PurchaseOrderCode;
        }
    }
}
