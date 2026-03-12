using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetGRNCodeHandler : IRequestHandler<GetGRNCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetGRNCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetGRNCodeQuery request, CancellationToken cancellationToken)
        {
            string _GRNCode = "";
            if (await unitOfWork.Repository<Entities.Models.GRN>().GetExistsAsync())
            {
                Func<IQueryable<Entities.Models.GRN>, IOrderedQueryable<Entities.Models.GRN>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var GRNCode = await unitOfWork.Repository<Entities.Models.GRN>().GetOneAsync(y => y.IsActive == true
                //&& y.Department.CompanyId == sessionProvider.Session.CompanyId
                , OrderByDesc, null);
                int No = Convert.ToInt32(GRNCode.Code) + 1;
                _GRNCode = No.ToString().PadLeft(7, '0');
            }
            else
                _GRNCode = "0000001";
          
            return _GRNCode;
        }
    }
}
