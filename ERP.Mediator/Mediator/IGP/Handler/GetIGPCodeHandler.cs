using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetIGPCodeHandler : IRequestHandler<GetIGPCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetIGPCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetIGPCodeQuery request, CancellationToken cancellationToken)
        {
            string _IGPCode = "";
            if (await unitOfWork.Repository<Entities.Models.IGP>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.IGP>, IOrderedQueryable<Entities.Models.IGP>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var IGPCode = await unitOfWork.Repository<Entities.Models.IGP>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(IGPCode.Code) + 1;
                _IGPCode = No.ToString().PadLeft(7, '0');
            }
            else
                _IGPCode = "0000001";
          
            return _IGPCode;
        }
    }
}
