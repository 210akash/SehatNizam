using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using ERP.Core.Provider;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class GetTransactionCodeHandler : IRequestHandler<GetTransactionCodeQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public GetTransactionCodeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(GetTransactionCodeQuery request, CancellationToken cancellationToken)
        {
            var VoucherType = await unitOfWork.Repository<Entities.Models.VoucherType>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.Id == request.VoucherTypeId, null, null);

            string _TransactionCode = "";
            if (await unitOfWork.Repository<Entities.Models.Transaction>().GetExistsAsync(y =>  y.CompanyId == sessionProvider.Session.CompanyId && y.VoucherTypeId == request.VoucherTypeId))
            {
                Func<IQueryable<Entities.Models.Transaction>, IOrderedQueryable<Entities.Models.Transaction>> OrderByDesc = query => query.OrderByDescending(x => x.Code);
                var TransactionCode = await unitOfWork.Repository<Entities.Models.Transaction>().GetOneAsync(y => y.IsActive == true && y.CompanyId == sessionProvider.Session.CompanyId && y.VoucherTypeId == request.VoucherTypeId, OrderByDesc, null);
                int No = Convert.ToInt32(TransactionCode.Code.Replace(VoucherType.Code, "")) + 1;
                _TransactionCode = No.ToString().PadLeft(7, '0');
            }
            else
                _TransactionCode = "0000001";
            return VoucherType.Code + _TransactionCode;
        }
    }
}
