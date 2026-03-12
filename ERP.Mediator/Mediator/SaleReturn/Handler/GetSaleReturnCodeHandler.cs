using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class GetSaleReturnCodeHandler : IRequestHandler<GetSaleReturnCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetSaleReturnCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetSaleReturnCodeQuery request, CancellationToken cancellationToken)
        {
            string _SaleReturnCode = "";
            if (await unitOfWork.Repository<Entities.Models.SaleReturn>().GetExistsAsync(y => y.IsActive))
            {
                Func<IQueryable<Entities.Models.SaleReturn>, IOrderedQueryable<Entities.Models.SaleReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var SaleReturnCode = await unitOfWork.Repository<Entities.Models.SaleReturn>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(SaleReturnCode.Code) + 1;
                _SaleReturnCode = No.ToString().PadLeft(7, '0');
            }
            else
                _SaleReturnCode = "0000001";
          
            return _SaleReturnCode;
        }
    }
}
