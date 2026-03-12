using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetInspectionCodeHandler : IRequestHandler<GetInspectionCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetInspectionCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetInspectionCodeQuery request, CancellationToken cancellationToken)
        {
            string _InspectionCode = "";
            if (await unitOfWork.Repository<Entities.Models.Inspection>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.Inspection>, IOrderedQueryable<Entities.Models.Inspection>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var InspectionCode = await unitOfWork.Repository<Entities.Models.Inspection>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(InspectionCode.Code) + 1;
                _InspectionCode = No.ToString().PadLeft(7, '0');
            }
            else
                _InspectionCode = "0000001";
          
            return _InspectionCode;
        }
    }
}
