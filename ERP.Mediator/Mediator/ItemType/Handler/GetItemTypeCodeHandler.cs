using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.ItemType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.ItemType.Handler
{
    public class GetItemTypeCodeHandler : IRequestHandler<GetItemTypeCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetItemTypeCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetItemTypeCodeQuery request, CancellationToken cancellationToken)
        {
            var SubCategory = await unitOfWork.Repository<Entities.Models.SubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.SubCategoryId);
            string _ItemTypeCode = "";
            if (await unitOfWork.Repository<Entities.Models.ItemType>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.SubCategoryId == request.SubCategoryId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.ItemType>, IOrderedQueryable<Entities.Models.ItemType>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var ItemTypeCode = await unitOfWork.Repository<Entities.Models.ItemType>().GetOneAsync(y => y.IsActive == true && y.SubCategoryId == request.SubCategoryId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(ItemTypeCode.Code.TakeLast(2).ToArray())) + 1;
                _ItemTypeCode = No.ToString().PadLeft(3, '0');
            }
            else
                _ItemTypeCode = "001";
          
            return SubCategory.Code + _ItemTypeCode;
        }
    }
}
