using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class GetRetailOrderReturnCodeHandler : IRequestHandler<GetRetailOrderReturnCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetRetailOrderReturnCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetRetailOrderReturnCodeQuery request, CancellationToken cancellationToken)
        {
            string _RetailOrderReturnCode = "";
            if (await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.RetailOrderReturn>, IOrderedQueryable<Entities.Models.RetailOrderReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var RetailOrderReturnCode = await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(RetailOrderReturnCode.Code) + 1;
                _RetailOrderReturnCode = No.ToString().PadLeft(7, '0');
            }
            else
                _RetailOrderReturnCode = "0000001";
          
            return _RetailOrderReturnCode;
        }
    }
}
