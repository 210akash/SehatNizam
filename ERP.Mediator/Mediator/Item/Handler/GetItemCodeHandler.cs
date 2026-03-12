using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Item.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Item.Handler
{
    public class GetItemCodeHandler : IRequestHandler<GetItemCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetItemCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetItemCodeQuery request, CancellationToken cancellationToken)
        {
            var ItemType = await unitOfWork.Repository<Entities.Models.ItemType>().GetFirstAsNoTrackingAsync(x => x.Id == request.ItemTypeId);
            string _ItemCode = "";
            if (await unitOfWork.Repository<Entities.Models.Item>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.ItemTypeId == request.ItemTypeId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.Item>, IOrderedQueryable<Entities.Models.Item>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ItemCode = await unitOfWork.Repository<Entities.Models.Item>().GetOneAsync(y => y.IsActive == true && y.ItemTypeId == request.ItemTypeId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(ItemCode.Code.TakeLast(4).ToArray())) + 1;
                _ItemCode = No.ToString().PadLeft(4, '0');
            }
            else
                _ItemCode = "0001";
          
            return ItemType.Code + _ItemCode;
        }
    }
}
