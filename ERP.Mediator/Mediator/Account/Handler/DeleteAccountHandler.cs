using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Account.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Account.Handler
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountQuery request, CancellationToken cancellationToken)
        {
            var Account = await unitOfWork.Repository<Entities.Models.Account>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Account.IsDelete = true;
            Account.IsActive = false;
            Account.DeleteDate = DateTime.Now;
            Account.ModifiedDate = DateTime.Now;
            Account.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Account>().Update(Account);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
