using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Ward.Query;

namespace ERP.Mediator.Mediator.IPD.Ward.Handler
{
    public class GetWardCodeHandler : IRequestHandler<GetWardCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetWardCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetWardCodeQuery request, CancellationToken cancellationToken)
        {
            string _WardCode = "";
            if (await unitOfWork.Repository<Entities.Models.Ward>().GetExistsAsync(y => y.ProjectId == sessionProvider.Session.SelectedWarehouseId && y.IsActive == true))
            {
                Func<IQueryable<Entities.Models.Ward>, IOrderedQueryable<Entities.Models.Ward>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var WardCode = await unitOfWork.Repository<Entities.Models.Ward>().GetOneAsync(y => y.IsActive == true && y.ProjectId == sessionProvider.Session.SelectedWarehouseId, OrderByDesc, null);
                int No = Convert.ToInt32(WardCode.Code) + 1;
                _WardCode = No.ToString().PadLeft(2, '0');
            }
            else
                _WardCode = "01";
          
            return _WardCode;
        }
    }
}
