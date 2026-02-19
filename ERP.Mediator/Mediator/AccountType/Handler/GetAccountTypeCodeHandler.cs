using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.AccountType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.AccountType.Handler
{
    public class GetAccountTypeCodeHandler : IRequestHandler<GetAccountTypeCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetAccountTypeCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetAccountTypeCodeQuery request, CancellationToken cancellationToken)
        {
            var AccountSubCategory = await unitOfWork.Repository<Entities.Models.AccountSubCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.AccountSubCategoryId);
            string _AccountTypeCode = "";
            if (await unitOfWork.Repository<Entities.Models.AccountType>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.AccountSubCategoryId == request.AccountSubCategoryId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.AccountType>, IOrderedQueryable<Entities.Models.AccountType>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var AccountTypeCode = await unitOfWork.Repository<Entities.Models.AccountType>().GetOneAsync(y => y.IsActive == true && y.AccountSubCategoryId == request.AccountSubCategoryId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(AccountTypeCode.Code.TakeLast(2).ToArray())) + 1;
                _AccountTypeCode = No.ToString().PadLeft(3, '0');
            }
            else
                _AccountTypeCode = "001";
          
            return AccountSubCategory.Code + _AccountTypeCode;
        }
    }
}
