using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Handler
{
    public class ProcessSaleReturnHandler : IRequestHandler<ProcessSaleReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessSaleReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessSaleReturnQuery request, CancellationToken cancellationToken)
        {
            var SaleReturn = await unitOfWork.Repository<Entities.Models.SaleReturn>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleReturn.StatusId = 2;
            SaleReturn.ProcessedDate = DateTime.Now;
            SaleReturn.ProcessedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SaleReturn>().Update(SaleReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
