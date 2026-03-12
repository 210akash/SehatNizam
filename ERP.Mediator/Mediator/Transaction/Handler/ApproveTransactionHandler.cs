using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class ApproveTransactionHandler : IRequestHandler<ApproveTransactionQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveTransactionHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveTransactionQuery request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Transaction.StatusId = 3;
            Transaction.ModifiedDate = DateTime.Now;
            Transaction.ModifiedById = sessionProvider.Session.LoggedInUserId;
            Transaction.ApprovedDate = DateTime.Now;
            Transaction.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Transaction>().Update(Transaction);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
