using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RetailOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RetailOrderReturn.Handler
{
    public class ProcessRetailOrderReturnHandler : IRequestHandler<ProcessRetailOrderReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessRetailOrderReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessRetailOrderReturnQuery request, CancellationToken cancellationToken)
        {
            var RetailOrderReturn = await unitOfWork.Repository<Entities.Models.RetailOrderReturn>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            RetailOrderReturn.StatusId = 3;
            RetailOrderReturn.ModifiedDate = DateTime.Now;
            RetailOrderReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RetailOrderReturn>().Update(RetailOrderReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
