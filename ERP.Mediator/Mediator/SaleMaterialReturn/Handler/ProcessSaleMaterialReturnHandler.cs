using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class ProcessSaleMaterialReturnHandler : IRequestHandler<ProcessSaleMaterialReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessSaleMaterialReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessSaleMaterialReturnQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterialReturn = await unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleMaterialReturn.StatusId = 2;
            SaleMaterialReturn.ProcessedDate = DateTime.Now;
            SaleMaterialReturn.ProcessedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().Update(SaleMaterialReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
