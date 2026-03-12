using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountGroup.Handler
{
    public class DeleteAccountGroupHandler : IRequestHandler<DeleteAccountGroupQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountGroupQuery request, CancellationToken cancellationToken)
        {
            var AccountGroup = await unitOfWork.Repository<Entities.Models.AccountGroup>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountGroup.IsDelete = true;
            AccountGroup.IsActive = false;
            AccountGroup.DeleteDate = DateTime.Now;
            AccountGroup.ModifiedDate = DateTime.Now;
            AccountGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountGroup>().Update(AccountGroup);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
