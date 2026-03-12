using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.SaleMaterial.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class GetSaleMaterialCodeHandler : IRequestHandler<GetSaleMaterialCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetSaleMaterialCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetSaleMaterialCodeQuery request, CancellationToken cancellationToken)
        {
            string _SaleMaterialCode = "";
            if (await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.SaleMaterial>, IOrderedQueryable<Entities.Models.SaleMaterial>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var SaleMaterialCode = await unitOfWork.Repository<Entities.Models.SaleMaterial>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(SaleMaterialCode.Code) + 1;
                _SaleMaterialCode = No.ToString().PadLeft(7, '0');
            }
            else
                _SaleMaterialCode = "0000001";
          
            return _SaleMaterialCode;
        }
    }
}
