using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class ApproveComparativeStatementHandler : IRequestHandler<ApproveComparativeStatementQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveComparativeStatementHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveComparativeStatementQuery request, CancellationToken cancellationToken)
        {
            var ComparativeStatement = await unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            ComparativeStatement.StatusId = 3;
            ComparativeStatement.ModifiedDate = DateTime.Now;
            ComparativeStatement.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ComparativeStatement>().Update(ComparativeStatement);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
