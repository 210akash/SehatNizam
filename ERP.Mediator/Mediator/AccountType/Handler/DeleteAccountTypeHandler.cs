using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountType.Handler
{
    public class DeleteAccountTypeHandler : IRequestHandler<DeleteAccountTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountTypeQuery request, CancellationToken cancellationToken)
        {
            var AccountType = await unitOfWork.Repository<Entities.Models.AccountType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountType.IsDelete = true;
            AccountType.IsActive = false;
            AccountType.DeleteDate = DateTime.Now;
            AccountType.ModifiedDate = DateTime.Now;
            AccountType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountType>().Update(AccountType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
