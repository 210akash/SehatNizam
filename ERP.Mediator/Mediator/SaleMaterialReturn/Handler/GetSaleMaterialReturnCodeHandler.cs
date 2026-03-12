using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class GetSaleMaterialReturnCodeHandler : IRequestHandler<GetSaleMaterialReturnCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetSaleMaterialReturnCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetSaleMaterialReturnCodeQuery request, CancellationToken cancellationToken)
        {
            string _SaleMaterialReturnCode = "";
            if (await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetExistsAsync(y => y.IsActive))
            {
                Func<IQueryable<Entities.Models.SaleMaterialReturn>, IOrderedQueryable<Entities.Models.SaleMaterialReturn>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var SaleMaterialReturnCode = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(SaleMaterialReturnCode.Code) + 1;
                _SaleMaterialReturnCode = No.ToString().PadLeft(7, '0');
            }
            else
                _SaleMaterialReturnCode = "0000001";
          
            return _SaleMaterialReturnCode;
        }
    }
}
