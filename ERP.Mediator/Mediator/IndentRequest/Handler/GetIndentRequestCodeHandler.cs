using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class GetIndentRequestCodeHandler : IRequestHandler<GetIndentRequestCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetIndentRequestCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetIndentRequestCodeQuery request, CancellationToken cancellationToken)
        {
            string _IndentRequestCode = "";
            if (await unitOfWork.Repository<Entities.Models.IndentRequest>().GetExistsAsync(y =>  y.Department.CompanyId == sessionProvider.Session.CompanyId))
            {
                Func<IQueryable<Entities.Models.IndentRequest>, IOrderedQueryable<Entities.Models.IndentRequest>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var IndentRequestCode = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetOneAsync(y => y.IsActive == true && y.Department.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                int No = Convert.ToInt32(IndentRequestCode.Code) + 1;
                _IndentRequestCode = No.ToString().PadLeft(7, '0');
            }
            else
                _IndentRequestCode = "0000001";
          
            return _IndentRequestCode;
        }
    }
}
