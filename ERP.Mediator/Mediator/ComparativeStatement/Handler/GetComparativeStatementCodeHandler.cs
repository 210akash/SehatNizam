using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class GetComparativeStatementCodeHandler : IRequestHandler<GetComparativeStatementCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetComparativeStatementCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetComparativeStatementCodeQuery request, CancellationToken cancellationToken)
        {
            string _ComparativeStatementCode = "";
            if (await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.ComparativeStatement>, IOrderedQueryable<Entities.Models.ComparativeStatement>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ComparativeStatementCode = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(ComparativeStatementCode.Code) + 1;
                _ComparativeStatementCode = No.ToString().PadLeft(7, '0');
            }
            else
                _ComparativeStatementCode = "0000001";
          
            return _ComparativeStatementCode;
        }
    }
}
