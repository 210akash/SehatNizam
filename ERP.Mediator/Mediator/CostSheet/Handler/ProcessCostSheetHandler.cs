using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CostSheet.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Handler
{
    public class ProcessCostSheetHandler : IRequestHandler<ProcessCostSheetQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessCostSheetHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessCostSheetQuery request, CancellationToken cancellationToken)
        {
            var CostSheet = await unitOfWork.Repository<Entities.Models.CostSheet>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            CostSheet.StatusId = 2;
            CostSheet.ModifiedDate = DateTime.Now;
            CostSheet.ModifiedById = sessionProvider.Session.LoggedInUserId;

            CostSheet.ProcessedDate = DateTime.Now;
            CostSheet.ProcessedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.CostSheet>().Update(CostSheet);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
