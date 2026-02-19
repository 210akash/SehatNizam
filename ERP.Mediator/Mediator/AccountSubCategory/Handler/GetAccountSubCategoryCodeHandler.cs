using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountSubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountSubCategory.Handler
{
    public class GetAccountSubCategoryCodeHandler : IRequestHandler<GetAccountSubCategoryCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetAccountSubCategoryCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountSubCategoryCodeQuery request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountCategoryId);
            string _AccountSubCategoryCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.AccountCategoryId == request.AccountCategoryId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.AccountSubCategory>, IOrderedQueryable<Entities.Models.AccountSubCategory>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountSubCategoryCode = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetOneAsync(y => y.IsActive == true && y.AccountCategoryId == request.AccountCategoryId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(AccountSubCategoryCode.Code.TakeLast(2).ToArray())) + 1;
                _AccountSubCategoryCode = No.ToString().PadLeft(2, '0');
            }
            else
                _AccountSubCategoryCode = "01";
          
            return Category.Code + _AccountSubCategoryCode;
        }
    }
}
