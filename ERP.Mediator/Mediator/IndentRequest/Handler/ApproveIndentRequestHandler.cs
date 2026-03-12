using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class ApproveIndentRequestHandler : IRequestHandler<ApproveIndentRequestQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveIndentRequestHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveIndentRequestQuery request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IndentRequest.StatusId = 3;
            IndentRequest.ModifiedDate = DateTime.Now;
            IndentRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;
            IndentRequest.ApprovedDate = DateTime.Now;
            IndentRequest.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IndentRequest>().Update(IndentRequest);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
