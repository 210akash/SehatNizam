using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.CostSheet.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.CostSheet.Handler
{
    public class GetCostSheetCodeHandler : IRequestHandler<GetCostSheetCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetCostSheetCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetCostSheetCodeQuery request, CancellationToken cancellationToken)
        {
            string _CostSheetCode = "";
            if (await unitOfWork.Repository<Entities.Models.CostSheet>().GetExistsAsync(y =>  y.Item.CompanyId == sessionProvider.Session.CompanyId))
            {
                Func<IQueryable<Entities.Models.CostSheet>, IOrderedQueryable<Entities.Models.CostSheet>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var CostSheetCode = await unitOfWork.Repository<Entities.Models.CostSheet>().GetOneAsync(y => y.IsActive == true && y.Item.CompanyId == sessionProvider.Session.CompanyId, OrderByDesc, null);
                int No = Convert.ToInt32(CostSheetCode.Code) + 1;
                _CostSheetCode = No.ToString().PadLeft(7, '0');
            }
            else
                _CostSheetCode = "0000001";
          
            return _CostSheetCode;
        }
    }
}
