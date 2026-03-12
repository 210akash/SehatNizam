using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.WarehouseTransfer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.WarehouseTransfer.Handler
{
    public class GetWarehouseTransferCodeHandler : IRequestHandler<GetWarehouseTransferCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetWarehouseTransferCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetWarehouseTransferCodeQuery request, CancellationToken cancellationToken)
        {
            string _WarehouseTransferCode = "";
            if (await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetExistsAsync(y=>y.IsActive))
            {
                Func<IQueryable<Entities.Models.WarehouseTransfer>, IOrderedQueryable<Entities.Models.WarehouseTransfer>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var WarehouseTransferCode = await unitOfWork.Repository<Entities.Models.WarehouseTransfer>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(WarehouseTransferCode.Code) + 1;
                _WarehouseTransferCode = No.ToString().PadLeft(7, '0');
            }
            else
                _WarehouseTransferCode = "0000001";
          
            return _WarehouseTransferCode;
        }
    }
}
