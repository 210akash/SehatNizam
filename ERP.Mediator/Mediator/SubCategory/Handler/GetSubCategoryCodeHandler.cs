using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class GetSubCategoryCodeHandler : IRequestHandler<GetSubCategoryCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetSubCategoryCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetSubCategoryCodeQuery request, CancellationToken cancellationToken)
        {
            var Category = await unitOfWork.Repository<Entities.Models.Category>().GetFirstAsNoTrackingAsync(x => x.Id == request.CategoryId);
            string _SubCategoryCode = "";
            if (await unitOfWork.Repository<Entities.Models.SubCategory>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.CategoryId == request.CategoryId && y.Id != request.Id))
            {
                Func<IQueryable<Entities.Models.SubCategory>, IOrderedQueryable<Entities.Models.SubCategory>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var SubCategoryCode = await unitOfWork.Repository<Entities.Models.SubCategory>().GetOneAsync(y => y.IsActive == true && y.CategoryId == request.CategoryId && y.CompanyId == sessionProvider.Session.CompanyId && y.Id != request.Id, OrderByDesc, null);
                int No = Convert.ToInt32(new string(SubCategoryCode.Code.TakeLast(2).ToArray())) + 1;
                _SubCategoryCode = No.ToString().PadLeft(2, '0');
            }
            else
                _SubCategoryCode = "01";
          
            return Category.Code + _SubCategoryCode;
        }
    }
}
