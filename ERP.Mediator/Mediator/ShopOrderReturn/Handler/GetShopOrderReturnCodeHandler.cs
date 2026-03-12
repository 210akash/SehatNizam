using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class GetShopOrderReturnCodeHandler : IRequestHandler<GetShopOrderReturnCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetShopOrderReturnCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetShopOrderReturnCodeQuery request, CancellationToken cancellationToken)
        {
            string _ShopOrderReturnCode = "";
            if (await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.ShopOrderReturn>, IOrderedQueryable<Entities.Models.ShopOrderReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ShopOrderReturnCode = await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(ShopOrderReturnCode.Code) + 1;
                _ShopOrderReturnCode = No.ToString().PadLeft(7, '0');
            }
            else
                _ShopOrderReturnCode = "0000001";
          
            return _ShopOrderReturnCode;
        }
    }
}
