using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetIssuanceCodeHandler : IRequestHandler<GetIssuanceCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetIssuanceCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetIssuanceCodeQuery request, CancellationToken cancellationToken)
        {
            string _IssuanceCode = "";
            if (await unitOfWork.Repository<Entities.Models.Issuance>().GetExistsAsync(y=>y.IsActive))
            {
                Func<IQueryable<Entities.Models.Issuance>, IOrderedQueryable<Entities.Models.Issuance>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var IssuanceCode = await unitOfWork.Repository<Entities.Models.Issuance>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(IssuanceCode.Code) + 1;
                _IssuanceCode = No.ToString().PadLeft(7, '0');
            }
            else
                _IssuanceCode = "0000001";
          
            return _IssuanceCode;
        }
    }
}
