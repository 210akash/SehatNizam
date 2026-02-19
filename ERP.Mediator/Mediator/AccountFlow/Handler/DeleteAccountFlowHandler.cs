using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountFlow.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountFlow.Handler
{
    public class DeleteAccountFlowHandler : IRequestHandler<DeleteAccountFlowQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountFlowHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountFlowQuery request, CancellationToken cancellationToken)
        {
            var AccountFlow = await unitOfWork.Repository<Entities.Models.AccountFlow>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountFlow.IsDelete = true;
            AccountFlow.IsActive = false;
            AccountFlow.DeleteDate = DateTime.Now;
            AccountFlow.ModifiedDate = DateTime.Now;
            AccountFlow.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountFlow>().Update(AccountFlow);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
