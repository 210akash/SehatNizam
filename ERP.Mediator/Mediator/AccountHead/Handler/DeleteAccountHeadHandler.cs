using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountHead.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountHead.Handler
{
    public class DeleteAccountHeadHandler : IRequestHandler<DeleteAccountHeadQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountHeadHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountHeadQuery request, CancellationToken cancellationToken)
        {
            var AccountHead = await unitOfWork.Repository<Entities.Models.AccountHead>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountHead.IsDelete = true;
            AccountHead.IsActive = false;
            AccountHead.DeleteDate = DateTime.Now;
            AccountHead.ModifiedDate = DateTime.Now;
            AccountHead.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountHead>().Update(AccountHead);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
