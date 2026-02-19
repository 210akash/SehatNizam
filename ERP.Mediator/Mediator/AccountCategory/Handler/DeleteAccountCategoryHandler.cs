using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AccountCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AccountCategory.Handler
{
    public class DeleteAccountCategoryHandler : IRequestHandler<DeleteAccountCategoryQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAccountCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAccountCategoryQuery request, CancellationToken cancellationToken)
        {
            var AccountCategory = await unitOfWork.Repository<Entities.Models.AccountCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AccountCategory.IsDelete = true;
            AccountCategory.IsActive = false;
            AccountCategory.DeleteDate = DateTime.Now;
            AccountCategory.ModifiedDate = DateTime.Now;
            AccountCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AccountCategory>().Update(AccountCategory);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
