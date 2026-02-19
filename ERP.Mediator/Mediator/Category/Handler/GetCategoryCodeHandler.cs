using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Category.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Category.Handler
{
    public class GetCategoryCodeHandler : IRequestHandler<GetCategoryCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetCategoryCodeHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(GetCategoryCodeQuery request, CancellationToken cancellationToken)
        {
            string _CategoryCode = "";
            if (await unitOfWork.Repository<Entities.Models.Category>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.Category>, IOrderedQueryable<Entities.Models.Category>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var CategoryCode = await unitOfWork.Repository<Entities.Models.Category>().GetOneAsync(y => y.IsActive == true, OrderByDesc, null);
                int No = Convert.ToInt32(CategoryCode.Code) + 1;
                _CategoryCode = No.ToString().PadLeft(2, '0');
            }
            else
                _CategoryCode = "01";
          
            return _CategoryCode;
        }
    }
}
