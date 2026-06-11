using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Service.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class GetServiceCodeHandler : IRequestHandler<GetServiceCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        public GetServiceCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(GetServiceCodeQuery request, CancellationToken cancellationToken)
        {
            string _ServiceCode = "";
            if (await unitOfWork.Repository<Entities.Models.Service>().GetExistsAsync(y => y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.Service>, IOrderedQueryable<Entities.Models.Service>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ServiceCode = await unitOfWork.Repository<Entities.Models.Service>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(ServiceCode.Code) + 1;
                _ServiceCode = No.ToString().PadLeft(2, '0');
            }
            else
                _ServiceCode = "01";
          
            return _ServiceCode;
        }
    }
}
