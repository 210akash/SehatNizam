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
    public class RejectCostSheetHandler : IRequestHandler<RejectCostSheetQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public RejectCostSheetHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(RejectCostSheetQuery request, CancellationToken cancellationToken)
        {
            var CostSheet = await unitOfWork.Repository<Entities.Models.CostSheet>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            CostSheet.StatusId = 1;
            CostSheet.ModifiedDate = DateTime.Now;
            CostSheet.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.CostSheet>().Update(CostSheet);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
