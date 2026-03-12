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
    public class ProcessIndentRequestHandler : IRequestHandler<ProcessIndentRequestQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessIndentRequestHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessIndentRequestQuery request, CancellationToken cancellationToken)
        {
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IndentRequest.StatusId = 2;
            IndentRequest.ModifiedDate = DateTime.Now;
            IndentRequest.ModifiedById = sessionProvider.Session.LoggedInUserId;

            IndentRequest.ProcessedDate = DateTime.Now;
            IndentRequest.ProcessedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.IndentRequest>().Update(IndentRequest);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
