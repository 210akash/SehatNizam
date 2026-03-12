using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Dispatch.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Dispatch.Handler
{
    public class GetCancelDispatchCodeHandler : IRequestHandler<GetCancelDispatchCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetCancelDispatchCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetCancelDispatchCodeQuery request, CancellationToken cancellationToken)
        {
            string _DispatchCode = "";
            if (await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetExistsAsync(x => x.IsActive))
            {
                Func<IQueryable<Entities.Models.CancelDispatch>, IOrderedQueryable<Entities.Models.CancelDispatch>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var DispatchCode = await unitOfWork.Repository<Entities.Models.CancelDispatch>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(DispatchCode.Code) + 1;
                _DispatchCode = No.ToString().PadLeft(7, '0');
            }
            else
                _DispatchCode = "0000001";
          
            return _DispatchCode;
        }
    }
}
